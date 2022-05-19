using System;
using System.Linq;

namespace CodeReview
{
    public static class Methods
    {
        //Назначение:
        //  Метод вставляет в переданный массив пар новую пару
        //и сортирует массив так, что вставленная пара 
        //оказывается последней, среди тех, 
        //у которых идентичный ключ.
        //
        //  Метод нарушает SRP. Он и вставляет элемент в массив,
        //и сортирует этот массив.Сделано это, скорее всего, для того, 
        //чтобы поддерживать массив в отсортированном состоянии, 
        //что необходимо для обеспечения быстрого поиска.
        //Тогда, правильнее будет: обеспечить вставку (перед первым большим или в конец), 
        //которая поддержит массив в отсортированном состоянии, 
        //что по сложности o(N) против o(n^2) пузырьковой сортировки,
        //либо вообще разделить метод на два метода, вставки и сортировки,
        //и вызывать их по отдельности.
        //  Важно, т.к. метод гипотетически может быть уже использован где-то в большом проекте,
        //будет весьма рискованно делать что-то из перечисленного. Поэтому,
        //я реализую две версии метода, так как будет наиболее правильно, и так, чтобы ничего не сломалось.
        
        public static void Func1(ref KeyValuePair<int, string>[] a, int key, string value)
        {
            Array.Resize(ref a, a.Length + 1);

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
        public static void SafetyRefactored(ref KeyValuePair<int, string>[] pairs, int key, string value)
        {
            pairs = pairs.Append(new KeyValuePair<int, string>(key, value))
                .OrderBy(e => e.Key)
                .ToArray();
        }
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
        public static IEnumerable<KeyValuePair<T1, T2>> AddToSortedAndReturn<T1, T2>
            (this IEnumerable<KeyValuePair<T1, T2>> pairs, KeyValuePair<T1, T2> toBeAdded)
            where T1 : IComparable<T1>
        {
            bool itsInsertedAlready = false;
            foreach (var p in pairs)
            {
                if (!itsInsertedAlready && p.Key.CompareTo(toBeAdded.Key) > 1)
                {
                    yield return toBeAdded;
                    itsInsertedAlready = true;
                    continue;
                }
                yield return p;
            }
            yield break;
        }

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
                //if (arr[i].Key > toCompare.Key)
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
            var func = Methods.AddToSortedArray<int, string>;

            var keyValuePairs = new List<KeyValuePair<int, string>>() {
                new KeyValuePair<int, string>(0, "should be first"),
                new KeyValuePair<int, string>(2, "should be third"),};

            var newPair = new KeyValuePair<int, string>(1, "should be second");

            var arrToPrint = keyValuePairs.ToArray();

            //func(ref arrToPrint, newPair.Key, newPair.Value);
            func(ref arrToPrint, newPair);

            PrintListOfPairs(arrToPrint);

            arrToPrint = GenerateListOfPairs(50, -10, 10).ToArray();

            //func(ref arrToPrint, newPair.Key, newPair.Value);
            func(ref arrToPrint, newPair);

            PrintListOfPairs(arrToPrint);



            Console.ReadLine();
        }


        
        



        static void PrintListOfPairs(IEnumerable<KeyValuePair<int, string>> keyValuePairs)
        {
            Console.WriteLine();
            foreach (var pair in keyValuePairs)
                Console.Write($"[{pair.Key}:{pair.Value}] ");
            Console.WriteLine();
        }


        static List<KeyValuePair<int, string>> GenerateListOfPairs(int size, int bottom, int top)
        {
            Random rnd = new Random();
            var result = new List<KeyValuePair<int, string>>(size);
            for (int i = 0; i < size; i++)
            {
                int randValue = rnd.Next(bottom, top);
                result.Add(new KeyValuePair<int, string>(randValue, randValue.ToString()));
            }
            return result;
        }
    }
}