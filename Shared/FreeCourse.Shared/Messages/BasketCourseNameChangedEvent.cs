namespace FreeCourse.Shared.Messages
{
    public class BasketCourseNameChangedEvent
    {
        public string UserId { get; set; }
        public string CourseId { get; set; }
        public string UpdatedName { get; set; }
    }
}