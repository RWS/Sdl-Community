using System.Collections.Generic;
using System.Linq;

namespace Sdl.Community.YourProductivity
{
    public static class Extensions
    {

        /// <summary>
        /// Gets the element that occurs most frequently in the collection.
        /// </summary>
        /// <param name="list"></param>
        /// <returns>Returns the element that occurs most frequently in the collection.
        /// If all elements occur an equal number of times, a random element in
        /// the collection will be returned.</returns>
        public static T Mode<T>(this IEnumerable<T> list)
        {
            // Initialize the return value
            T mode = default(T);

            // Test for a null reference and an empty list
            if (list != null && list.Any())
            {
                // Store the number of occurences for each element
                var counts = new Dictionary<T, int>();

                // Add one to the count for the occurence of a character
                foreach (T element in list)
                {
                    if (counts.ContainsKey(element))
                        counts[element]++;
                    else
                        counts.Add(element, 1);
                }

                // Loop through the counts of each element and find the 
                // element that occurred most often
                int max = 0;

                foreach (KeyValuePair<T, int> count in counts)
                {
                    if (count.Value > max)
                    {
                        // Update the mode
                        mode = count.Key;
                        max = count.Value;
                    }
                }
            }

            return mode;
        }
    }
}
