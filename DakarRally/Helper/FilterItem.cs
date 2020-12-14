namespace DakarRally.Helper
{
    /// <summary>
    /// FilterItem class.
    /// </summary>
    public class FilterItem
    {
        /// <summary>
        /// Field value specificied for filtering.
        /// </summary>
        public string Field { get; set; }

        /// <summary>
        /// Logic operation (AND/OR).
        /// </summary>
        public string LogicOperation { get; set; }
    }
}
