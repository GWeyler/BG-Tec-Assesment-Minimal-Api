using BG_Tec_Assesment_Minimal_Api.Utils;

namespace BG_Tec_Assesment_Minimal_Api.DTO
{
    public class RequestResult<T>
    {

        public ErrorEnum ErrorCode { get; set; }

        public string? Message { get; set; }

        public T? Value { get; set; }
    }
}
