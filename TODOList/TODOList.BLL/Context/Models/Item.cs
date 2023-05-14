namespace TODOList.Business.Context.Models
{
    /// <summary>
    /// Item of task in the TODO list
    /// </summary>
    public class Item
    {
        /// <summary>
        /// ID of the task
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Task name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Is the task done or not
        /// </summary>
        public bool IsCompleted { get; set; }
    }
}
