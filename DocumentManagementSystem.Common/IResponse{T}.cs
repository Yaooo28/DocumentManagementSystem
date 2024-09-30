using System.Collections.Generic;

namespace DocumentManagementSystem.Common
{
    public interface IResponse<T> : IResponse
    {
        T Data { get; set; }

        List<CustomValidationError> ValidationErrors { get; set; }
    }
}
