using System;
using System.Collections.Generic;

namespace Business.Services.Interfaces
{
    /// <summary>
    /// IWordsCounterService
    /// </summary>
    public interface IWordsCounterService
    {
        /// <summary>
        /// Count how many times words occur in specific text
        /// </summary>
        /// <param name="text">Text</param>
        /// <param name="isRegisterSensitive">Should be register sensitive?</param>
        /// <returns>Words</returns>
        Dictionary<string, int> CountAllWordsIn(string text, bool isRegisterSensitive = true);
    }
}
