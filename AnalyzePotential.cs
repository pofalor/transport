using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transport
{
    public static class AnalyzePotential
    {
        public static int[][] GetOptimizedSolution()
        {

            //1. Найти отрицательный наименьший элемент. Запоминаем, что это за ячейка и вызывааем метод оптимизации по ячейке

            //2. Проверяем есть ли отрицательные элементы, если есть, повторяем алгоритм
            var a = new int[1][];
            return a;
        }

        public static int[][] OptimizeSolution(Element[][] transportPlan, int N, int M)
        {
            ConcurrentBag<Element> bagElements = new ConcurrentBag<Element>();

            //TODO: пока я ищу сам ячейку с минимальным значением, в будущем надо будет передавать в метод индекс

            //Выбираем ячейку с самым маленьким значением
            Parallel.For(0, N, i => 
            {
                Element minElement = new Element()
                {
                    Potential = int.MaxValue
                };
                for (int j = 0; j < M; j++) 
                {
                    var element = transportPlan[i][j];
                    element.IndexCol = j;
                    element.IndexRow = i;
                    if(element.Potential.HasValue && element.Potential <  minElement.Potential)
                        minElement = element;
                }
                bagElements.Add(minElement);
            });

            var minElem = new Element()
            {
                Potential = int.MaxValue
            };
            foreach(var element in bagElements)
            {
                if(element.Potential.HasValue && element.Potential < minElem.Potential)
                    minElem = element;
            }

            //3. Цикл перераспределения поставок
            var allTreeElements = new Dictionary<Element, TreeForOptimize>();
            var tree = new TreeForOptimize(minElem, transportPlan, allTreeElements, N, M);
            var allPaths = new List<List<TreeForOptimize>>();
            var path = new List<TreeForOptimize>();
            // 3.1 Найти путь, по которому можно пройти так, чтобы можно было создать замкнутый круг и двигаться можно только влево, вправо, вверх, вниз
            GetLongestCyclePathForElement(tree, tree, path, allPaths);
            var maxLenTreeList = new List<TreeForOptimize>();
            foreach (var item in allPaths)
            {
                if (item.Count > maxLenTreeList.Count)
                    maxLenTreeList = item;
            }
            //3.2 После этого переходим к ячейке, которую нужно добавить в решение


            //3.3. смотрим две связанные ячейки друг с другм. Берём мин значение из двух. 
            //3.4. В новую ячейку ставим мин. значение
            //3.5. Корректируем значения во всех соответствующих ячейках, при этом если двигаемся вниз или вверх, то корректируем по столбцу. Если влево или вправо, то корректируем по строке.
            return new int[1][];
        }

        public static void GetLongestCyclePathForElement(TreeForOptimize currentElement, TreeForOptimize searchingElement, List<TreeForOptimize> path,
            List<List<TreeForOptimize>> allPaths)
        {
            path.Add(currentElement);
            var left = currentElement.LeftElement;
            var right = currentElement.RightElement;
            var top = currentElement.TopElement;
            var down = currentElement.DownElement;
            if (left != null)
            {
                if (left != searchingElement)
                {
                    GetLongestCyclePathForElement(left, searchingElement, path, allPaths);
                }
                else
                {
                    allPaths.Add(path);
                }
            }
            if (right != null)
            {
                if (right != searchingElement)
                {
                    GetLongestCyclePathForElement(right, searchingElement, path, allPaths);
                }
                else
                {
                    allPaths.Add(path);
                }
            }
            if (top != null)
            {
                if (top != searchingElement)
                {
                    GetLongestCyclePathForElement(top, searchingElement, path, allPaths);
                }
                else
                {
                    allPaths.Add(path);
                }
            }
            if (down != null)
            {
                if (down != searchingElement)
                {
                    GetLongestCyclePathForElement(down, searchingElement, path, allPaths);
                }
                else
                {
                    allPaths.Add(path);
                }
            }
        }
    }
}

