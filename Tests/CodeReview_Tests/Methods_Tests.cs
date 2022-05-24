using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using CodeReview;

namespace Tests.CodeReview_Tests
{
    [TestFixture]
    internal class Methods_Tests
    {
        private static readonly object[] TestCases =
        {
            new int[]{100, -100, 100, 101 },
            new int[]{100, -100, 100, -101 },
            new int[]{10000, -100, 100, 0 },
            new int[]{100, -100, 100, 1 },
            new int[]{100, -100, 100, -1 },
        };

        [Test, TestCaseSource("TestCases")]
        public void Func1_LongNotSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomNotSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound).ToArray();
            var expectedReslut = randomNotSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.Func1(ref randomNotSortedArr, newKey, "new pair");

            bool equivalence = randomNotSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }

        [Test]
        public void Func1_Empty_ContainsInsertedPair()
        {
            var arr = new KeyValuePair<int, string>[]{ };
            var expectedLegth = 1;
            var expectedKey = 11;
            var expectedValue = "new pair";

            CodeReview.Methods.Func1(ref arr, expectedKey, expectedValue);

            Assert.AreEqual(expectedLegth, arr.Length);
            Assert.AreEqual(expectedKey, arr[0].Key);
            Assert.AreEqual(expectedValue, arr[0].Value);
        }

        [Test, TestCaseSource("TestCases")]
        public void SafetyRefactored_LongNotSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomNotSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound).ToArray();
            var expectedReslut = randomNotSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.SafetyRefactored(ref randomNotSortedArr, newKey, "new pair");

            bool equivalence = randomNotSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }

        [Test]
        public void SafetyRefactored_Empty_ContainsInsertedPair()
        {
            var arr = new KeyValuePair<int, string>[] { };
            var expectedLegth = 1;
            var expectedKey = 11;
            var expectedValue = "new pair";

            CodeReview.Methods.SafetyRefactored(ref arr, expectedKey, expectedValue);

            Assert.AreEqual(expectedLegth, arr.Length);
            Assert.AreEqual(expectedKey, arr[0].Key);
            Assert.AreEqual(expectedValue, arr[0].Value);
        }

        [Test, TestCaseSource("TestCases")]
        public void AddToSortedArray_LongSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound)
                .OrderBy(e => e.Key)
                .ToArray();
            var expectedReslut = randomSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            CodeReview.Methods.AddToSortedArray(ref randomSortedArr, new KeyValuePair<int, string>(newKey, "new pair"));

            bool equivalence = randomSortedArr
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }

        [Test]
        public void AddToSortedArray_Empty_ContainsInsertedPair()
        {
            var arr = new KeyValuePair<int, string>[] { };
            var expectedLegth = 1;
            var expectedPair = new KeyValuePair<int, string>(11, "new pair");

            CodeReview.Methods.AddToSortedArray(ref arr, expectedPair);

            Assert.AreEqual(expectedLegth, arr.Length);
            Assert.AreEqual(expectedPair, arr[0]);
        }


        [Test, TestCaseSource("TestCases")]
        public void AddToSortedAndReturn_LongSortedRandomArr_EqualToSorted(int arrSize, int bottomBound, int topBound, int newKey)
        {
            var randomSortedArr = GenerateListOfPairs(arrSize, bottomBound, topBound)
                .OrderBy(e => e.Key);
            var expectedReslut = randomSortedArr
                .Append(new KeyValuePair<int, string>(newKey, "new pair"))
                .OrderBy(e => e.Key)
                .ToArray();

            bool equivalence = randomSortedArr
                .AddToSortedAndReturn(new KeyValuePair<int, string>(newKey, "new pair")) // Act is here
                .Select((elem, i) => elem.ToString() == expectedReslut[i].ToString())
                .All(res => res == true);
            Assert.True(equivalence, "Sequences were not equal");
        }

        [Test]
        public void AddToSortedAndReturn_Empty_ContainsInsertedPair()
        {
            var arr = new KeyValuePair<int, string>[] { };
            var expectedLegth = 1;
            var expectedPair = new KeyValuePair<int, string>(11, "new pair");

            arr = arr.AddToSortedAndReturn(expectedPair).ToArray();

            Assert.AreEqual(expectedLegth, arr.Length);
            Assert.AreEqual(expectedPair, arr.FirstOrDefault());
        }

        private static List<KeyValuePair<int, string>> GenerateListOfPairs(int size, int bottom, int top)
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
