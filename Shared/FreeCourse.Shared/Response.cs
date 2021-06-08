using System.Collections.Generic;
using System.Net;
using System.Text.Json.Serialization;
using FreeCourse.Shared.Dtos;

namespace FreeCourse.Shared
{
    public class Response<T>
    {
        public T Data { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
        [JsonIgnore]
        public bool IsSuccessfull { get; set; }

        public List<string> Errors { get; set; }
        
        //Static Factory Method
        public static Response<T> Success(T data, HttpStatusCode statusCode)
        {
            return new Response<T> {Data = data, StatusCode = statusCode, IsSuccessfull = true};
        }
        
        public static Response<T> Success(HttpStatusCode statusCode)
        {
            return new Response<T> {Data = default(T), StatusCode = statusCode,IsSuccessfull = true};
        }

        public static Response<T> Fail(List<string> errors, HttpStatusCode statusCode)
        {
            return new Response<T> {Errors = errors, StatusCode = statusCode, IsSuccessfull = false};
        }

        public static Response<T> Fail(string error, HttpStatusCode statusCode)
        {
            return new Response<T> {Errors = new List<string>() {error}, StatusCode = statusCode, IsSuccessfull = false};
        }
        
        public static Response<NoContent> NoContent()
        {
            return new Response<NoContent> {StatusCode = HttpStatusCode.NoContent};
        }
        
    }
}