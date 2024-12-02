using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport
{
    public class TreeForOptimize
    {
        public TreeForOptimize(Element currentElement, Element[][] transportPlan, Dictionary<Element, TreeForOptimize> allTreeElements, int N, int M)
        {
            CurrentElement = currentElement;

            allTreeElements[currentElement] = this;

            var downElement = N > currentElement.IndexRow + 1 ? GetElementWithOffset(transportPlan, currentElement.IndexRow, currentElement.IndexCol, 1) : null;
            if (downElement != null)
            {
                DownElement = allTreeElements.Get(downElement, new TreeForOptimize(downElement, transportPlan, allTreeElements, N, M));
            }

            var topElement = N - 1 >= 0 ? GetElementWithOffset(transportPlan, currentElement.IndexRow, currentElement.IndexCol, -1) : null;
            if (topElement != null)
            {
                TopElement = allTreeElements.Get(topElement, new TreeForOptimize(topElement, transportPlan, allTreeElements, N, M));
            }

            var leftElement = currentElement.IndexCol - 1 >= 0 ? GetElementWithOffset(transportPlan, currentElement.IndexRow, currentElement.IndexCol, offsetCol: -1) : null;
            if (leftElement != null)
            {
                LeftElement = allTreeElements.Get(leftElement, new TreeForOptimize(leftElement, transportPlan, allTreeElements, N, M));
            }

            var rightElement = M > currentElement.IndexCol + 1 ? GetElementWithOffset(transportPlan, currentElement.IndexRow, currentElement.IndexCol, offsetCol: 1) : null;
            if (rightElement != null)
            {
                RightElement = allTreeElements.Get(rightElement, new TreeForOptimize(rightElement, transportPlan, allTreeElements, N, M));
            }
        }

        public Element CurrentElement { get; set; }

        public TreeForOptimize? DownElement { get; set; } = null;

        public TreeForOptimize? TopElement { get; set; } = null;

        public TreeForOptimize? LeftElement { get; set; } = null;

        public TreeForOptimize? RightElement { get; set; } = null;

        private static Element GetElementWithOffset(Element[][] transportPlan, int indexRow, int indexCol, int? offsetRow = null, int? offsetCol = null)
        {
            var element = transportPlan[indexRow + offsetRow.GetValueOrDefault(0)][indexCol + offsetCol.GetValueOrDefault(0)];
            if (element.Weight == -1 || element.Weight == 0)
            {
                if (offsetRow.HasValue)
                {
                    offsetRow += offsetRow;
                }
                if (offsetCol.HasValue)
                {
                    offsetCol += offsetCol;
                }
                element = GetElementWithOffset(transportPlan, indexRow, indexCol, offsetRow, offsetCol);
            }
            return element;
        }

    }

    public static class DictionaryExtensions
    {
        /// <summary>
        /// Получить значение словаря по ключу, в случае отсутствия ключа вернет default
        /// </summary>
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue defaultValue)
        {
            if (source.ContainsKey(key))
            {
                return source[key];
            }
            return defaultValue;
        }
    }
}
