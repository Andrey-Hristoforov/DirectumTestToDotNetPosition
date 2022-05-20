using System;
using System.Linq;

namespace CodeReview
{
    public static class Methods
    {
        // Назначение:
        //   Метод вставляет в переданный массив пар новую пару
        // и сортирует массив так, что вставленная пара 
        // оказывается последней, среди тех, 
        // у которых идентичный ключ.
        // Вставка занимает временную сложность o(N).
        // Сортировка имеет временную сложность o(N^2).
        // 
        //   Метод нарушает SRP. Он и вставляет элемент в массив,
        // и сортирует этот массив. Сделано это, скорее всего, для того, 
        // чтобы поддерживать массив в отсортированном состоянии, 
        // что необходимо для обеспечения быстрого поиска.
        // Тогда, правильнее будет: обеспечить вставку (перед первым большим или в конец), 
        // которая поддержит массив в отсортированном состоянии, 
        // что по сложности o(N) против o(n^2) пузырьковой сортировки,
        // либо вообще разделить метод на два метода, вставки и сортировки,
        // и вызывать их по отдельности.
        //   Важно, т.к. метод гипотетически может быть уже использован где-то в большом проекте,
        // будет весьма рискованно делать что-то из перечисленного. Поэтому,
        // я реализую три версии метода, две, так, как будет правильнее на мой взгляд, и одну, так,
        // чтобы ничего не сломалось.
        // 
        // Недостатки метода:
        //   1)Неинформативное название метода.
        //   2)Раздельное указание key и value в сигнатуре метода.
        // Лучше указать сразу keyValuePair.
        //   3)Не информативное наименование переменной "a". Лучше pairs, pairsCollection
        // pairsArr и т.д.
        //   4)Ненужное объявление переменной keyValuePair, которая понятности коду не добавляет,
        // и нигде больше не встречается.
        //   5)Пузырьковая сортировка имеет квадратную временную сложность,
        // и больше подходит для изучения алгоритмов сортировки.
        // Вместо самодельной пузырьковой лучше использовать:
        // либо OrderBy из LINQ, который вроде как не реализует быструю сортировку,
        // т.к. он ленивый, но все равно уровень доверия к нему выше и вероятность бага в нем
        // значительно меньше(является реализацией стабильной сортировки),
        // либо использовать быструю сортировку (оказывается нет,
        // т.к. быстрая сортировка не обеспечивает стабильность сортировки,
        // т.е. не сохраняет порядок равных элементов).
        //   6)обход с конца не оправдан, и сбивает с толку, изучающего код человека.
        //   7)Вместо x лучше использовать название temp.
        // 
        // Не то чтобы недостатки, скорее пожелания:
        //   8)Привычнее будет реализация не через ref а через возврат результата в качестве
        // результата работы метода. Можно задуматься о ленивой реализации через yield, 
        // и реализации как метода-расширения.
        //   9)Можно изменить принимаемый тип с массива на IEnumerable.
        //   10)Можно добавить переменные типа, для добавления методу универсальности.
        //   11)Array.Resize не так узнаваем как linq Append(e).ToArray()

        public static void Func1(ref KeyValuePair<int, string>[] a, int key, string value)// 1, 2, 3, 8, 9, 10
        {
            Array.Resize(ref a, a.Length + 1);// 11

            var keyValuePair = new KeyValuePair<int, string>(key, value);
            a[a.Length - 1] = keyValuePair;

            for (int i = 0; i < a.Length; i++)
            {
                for (int j = a.Length - 1; j > 0; j--)
                {
                    if (a[j - 1].Key > a[j].Key)
                    {
                        KeyValuePair<int, string> x;
                        x = a[j - 1];
                        a[j - 1] = a[j];
                        a[j] = x;
                    }
                }
            }
        }

        // Безопасная реализация, которая работет не хуже предложенной. Безопасна она,
        // в том смысле, что замена предложенной на эту, не сломает работу кода,
        // который использует предложенный метод.
        public static void SafetyRefactored(ref KeyValuePair<int, string>[] pairs, int key, string value)
        {
            pairs = pairs.Append(new KeyValuePair<int, string>(key, value))
                .OrderBy(e => e.Key)
                .ToArray();
        }
        // Усовершенствованная версия метода, которая не сортирует массив,
        // а лишь вставляет новую пару в соотвествующее ей место в отсортированном массиве.
        // также может принимать массив пар любых типов.
        public static void AddToSortedArray<T1, T2>(ref KeyValuePair<T1, T2>[] pairs, KeyValuePair<T1, T2> toBeAdded) 
            where T1 : IComparable<T1>
        {
            Array.Resize(ref pairs, pairs.Length + 1);

            int indexOfFirstGreater = FindIndexOfFirstGreater(pairs, toBeAdded);
            if(indexOfFirstGreater == -1)
            {
                pairs[pairs.Length - 1] = toBeAdded;
                return;
            }

            pairs.MoveRightOneStepExceptLastStartingAtIndex(indexOfFirstGreater);

            pairs[indexOfFirstGreater] = toBeAdded;
        }
        // Наилучшая по моему мнению версия метода. Ленивая реализация, можно использовать в LINQ цепочках.
        // Не уступает остальным по производительности.
        public static IEnumerable<KeyValuePair<T1, T2>> AddToSortedAndReturn<T1, T2>
            (this IEnumerable<KeyValuePair<T1, T2>> pairs, KeyValuePair<T1, T2> toBeAdded)
            where T1 : IComparable<T1>
        {
            bool itsInsertedAlready = false;
            foreach (var p in pairs)
            {
                if (!itsInsertedAlready && p.Key.CompareTo(toBeAdded.Key) > 0)
                {
                    yield return toBeAdded;
                    itsInsertedAlready = true;
                }
                yield return p;
            }
            yield break;
        }

        // Вспомогательные методы
        private static void MoveRightOneStepExceptLastStartingAtIndex<T1, T2>(this KeyValuePair<T1, T2>[] arr, int startIndex)
            where T1 : IComparable<T1>
        {
            for (int i = arr.Length - 1; i > startIndex; i--)
            {
                arr[i] = arr[i - 1];
            }
        }
        private static int FindIndexOfFirstGreater<T1, T2>(KeyValuePair<T1, T2>[] arr, KeyValuePair<T1, T2> toCompare)
            where T1 : IComparable<T1>
        {
            int indexOfFirstGreater = -1;
            for (int i = 0; i < arr.Length; i++)
                if(arr[i].Key.CompareTo(toCompare.Key) > 0)
                {
                    indexOfFirstGreater = i;
                    break;
                }
            return indexOfFirstGreater;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        { 
        }
    }
}