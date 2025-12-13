using Appointments.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Appointments.Application.Results
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public IReadOnlyList<ValidationError> Errors { get; }

        private Result(T value)
        {
            IsSuccess = true;
            Value = value;
            Errors = [];
        }

        private Result(IEnumerable<ValidationError> errors)
        {
            IsSuccess = false;
            Errors = errors.ToList();
        }

        public static Result<T> Success(T value) => new(value);

        public static Result<T> Failure(params ValidationError[] errors)
            => new(errors);
    }
}
