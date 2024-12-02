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

            var downElement = N > currentElement.IndexRow + 1 ? GetElementWithOffset(transportPlan, N, M, currentElement.IndexRow, currentElement.IndexCol, 1) : null;
            if (downElement != null)
            {
                DownElement = allTreeElements.ContainsKey(downElement) ? allTreeElements[downElement] : new TreeForOptimize(downElement, transportPlan, allTreeElements, N, M);
            }

            var topElement = N - 1 >= 0 ? GetElementWithOffset(transportPlan, N, M, currentElement.IndexRow, currentElement.IndexCol, -1) : null;
            if (topElement != null)
            {
                TopElement = allTreeElements.ContainsKey(topElement) ? allTreeElements[topElement] : new TreeForOptimize(topElement, transportPlan, allTreeElements, N, M);
            }

            var leftElement = currentElement.IndexCol - 1 >= 0 ? GetElementWithOffset(transportPlan, N, M, currentElement.IndexRow, currentElement.IndexCol, offsetCol: -1) : null;
            if (leftElement != null)
            {
                LeftElement = allTreeElements.ContainsKey(leftElement) ? allTreeElements[leftElement] : new TreeForOptimize(leftElement, transportPlan, allTreeElements, N, M);
            }

            var rightElement = M > currentElement.IndexCol + 1 ? GetElementWithOffset(transportPlan, N, M, currentElement.IndexRow, currentElement.IndexCol, offsetCol: 1) : null;
            if (rightElement != null)
            {
                RightElement = allTreeElements.ContainsKey(rightElement) ? allTreeElements[rightElement] : new TreeForOptimize(rightElement, transportPlan, allTreeElements, N, M);
            }
        }

        public Element CurrentElement { get; set; }

        public TreeForOptimize? DownElement { get; set; } = null;

        public TreeForOptimize? TopElement { get; set; } = null;

        public TreeForOptimize? LeftElement { get; set; } = null;

        public TreeForOptimize? RightElement { get; set; } = null;

        private static Element? GetElementWithOffset(Element[][] transportPlan, int N, int M, int indexRow, int indexCol, int? offsetRow = null, int? offsetCol = null)
        {
            var resultCol = indexCol + offsetCol.GetValueOrDefault(0);
            var resultRow = indexRow + offsetRow.GetValueOrDefault(0);
            if (resultCol < 0 || resultCol >= M || resultRow < 0 || resultRow >= N)
            {
                return null;
            }
            var element = transportPlan[resultRow][resultCol];
            if (element != null && (element != CacheTree.currentElement || !element.IsPotentialNegative) && (element.Weight == -1 || element.Weight == 0))
            {
                if (offsetRow.HasValue)
                {
                    offsetRow += offsetRow;
                }
                if (offsetCol.HasValue)
                {
                    offsetCol += offsetCol;
                }
                element = GetElementWithOffset(transportPlan, N, M, indexRow, indexCol, offsetRow, offsetCol);
            }
            return element;
        }

    }

    public static class CacheTree
    {
        public static Element currentElement;
    }
}
