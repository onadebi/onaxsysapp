namespace AppCore.GenericRepo
{
    public class Pagination
    {
        public int PageCount { get; set; } = 250;
        public int PageNumber { get; set; } = 1;

        public Pagination()
        {
            if (PageCount > 1000)
            {
                PageCount = 1000;
            }
        }
        /// <summary>
        /// Assign page number and page count. Max page count is 1000.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageCount"></param>
        public Pagination(int pageNumber, int pageCount)
        {
            PageNumber = pageNumber;
            PageCount = pageCount > 1000 ? 1000 : pageCount;
        }
    }
}
