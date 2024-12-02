using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Transport
{
    public static class AnalyzePotential
    {
        public static void OptimizeSolution(Element[][] transportPlan, int N, int M, int?[] rowPotentials, int?[] colPotentials)
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
            CacheTree.currentElement = minElem;
            var tree = new TreeForOptimize(minElem, transportPlan, allTreeElements, N, M);
            var allPaths = new List<List<TreeForOptimize>>();
            var path = new List<TreeForOptimize>();
            // 3.1 Найти путь, по которому можно пройти так, чтобы можно было создать замкнутый круг и двигаться можно только влево, вправо, вверх, вниз
            GetAllCyclePathsForElement(tree, tree, tree, path, allPaths);
            var minLenTreeList = new List<TreeForOptimize>();
            foreach (var item in allPaths)
            {
                if (minLenTreeList.Count == 0 || item.Count < minLenTreeList.Count)
                    minLenTreeList = item;
            }
            //3.2 После этого переходим к ячейке, которую нужно добавить в решение
            var searchingTree = minLenTreeList[0];
            //TODO: изменить направление если что
            //3.3. смотрим две связанные ячейки друг с другм. Берём мин значение из двух.
            var prevCell = minLenTreeList[minLenTreeList.Count - 1].CurrentElement.Weight < minLenTreeList[1].CurrentElement.Weight ?
                minLenTreeList[minLenTreeList.Count - 1] : minLenTreeList[1];
            //3.4. В новую ячейку ставим мин. значение
            searchingTree.CurrentElement.Weight = prevCell.CurrentElement.Weight;

            BalanceElements(transportPlan, N, M, rowPotentials, colPotentials, minLenTreeList);
        }

        private static void BalanceElements(Element[][] transportPlan, int N, int M, int?[] rowPotentials, int?[] colPotentials, 
            List<TreeForOptimize> path)
        {
            //3.5. Корректируем значения во всех соответствующих ячейках, при этом если двигаемся вниз или вверх, то корректируем по столбцу. Если влево или вправо, то корректируем по строке.
            var prevCell = path[0];
            var counter = 0;
            //скипаем нулевой элемент, т.к. мы его вес уже поменяли
            foreach (var item in path.Skip(1)) 
            {
                var isFromLeftRight = item == prevCell.RightElement || item == prevCell.LeftElement;

                if (isFromLeftRight)
                {
                    var correctionSumForCurrentElement = 0;
                    for (int i = 0; i < N; i++) 
                    {
                        if(i == item.CurrentElement.IndexRow)
                        {
                            //текущий элемент пропускаем, т.к. мы его балансируем
                            continue;
                        }
                        var element = transportPlan[i][item.CurrentElement.IndexCol];
                        if(element.Weight != 0)
                        {
                            correctionSumForCurrentElement += element.Weight;
                        }
                    }
                    item.CurrentElement.Weight = colPotentials[item.CurrentElement.IndexCol] - correctionSumForCurrentElement ?? 0;
                }
                else
                {
                    var correctionSumForCurrentElement = 0;
                    for (int i = 0; i < M; i++)
                    {
                        if (i == item.CurrentElement.IndexCol)
                        {
                            //текущий элемент пропускаем, т.к. мы его балансируем
                            continue;
                        }
                        var element = transportPlan[item.CurrentElement.IndexRow][i];
                        if (element.Weight != 0)
                        {
                            correctionSumForCurrentElement += element.Weight;
                        }
                    }
                    item.CurrentElement.Weight = rowPotentials[item.CurrentElement.IndexRow] - correctionSumForCurrentElement ?? 0;
                }
                counter++;
                prevCell = path[counter];
            }
        }

        private static bool GetAllCyclePathsForElement(TreeForOptimize currentElement, TreeForOptimize prevElement, TreeForOptimize searchingElement, List<TreeForOptimize> path,
            List<List<TreeForOptimize>> allPaths)
        {
            path.Add(currentElement);
            var left = currentElement.LeftElement;
            var right = currentElement.RightElement;
            var top = currentElement.TopElement;
            var down = currentElement.DownElement;
            //если пришли слева, то обратно идти не надо
            if (left != null && prevElement.RightElement != currentElement)
            {
                if (left != searchingElement)
                {
                    var res = GetAllCyclePathsForElement(left, currentElement, searchingElement, path, allPaths);
                    if (res)
                        return res;
                }
                else
                {
                    path.Remove(currentElement);
                    allPaths.Add(path);
                    return true;
                }
            }
            if (right != null && prevElement.LeftElement != currentElement)
            {
                if (right != searchingElement)
                {
                    var res = GetAllCyclePathsForElement(right, currentElement, searchingElement, path, allPaths);
                    if (res)
                        return res;
                }
                else
                {
                    path.Remove(currentElement);
                    allPaths.Add(path);
                    return true;
                }
            }
            if (top != null && prevElement.DownElement != currentElement)
            {
                if (top != searchingElement)
                {
                    var res = GetAllCyclePathsForElement(top, currentElement, searchingElement, path, allPaths);
                    if (res)
                        return res;
                }
                else
                {
                    path.Remove(currentElement);
                    allPaths.Add(path);
                    return true;
                }
            }
            if (down != null && prevElement.TopElement != currentElement)
            {
                if (down != searchingElement)
                {
                    var res = GetAllCyclePathsForElement(down, currentElement,  searchingElement, path, allPaths);
                    if (res)
                        return res;
                }
                else
                {
                    path.Remove(currentElement);
                    allPaths.Add(path);
                    return true;
                }
            }
            return false;
        }
    }
}

