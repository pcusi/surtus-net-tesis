using System;

namespace surtus_api_restful.Models
{
    public interface IIdAutogenerado<T> where T : struct, IComparable, IComparable<T>, IConvertible, IEquatable<T>, IFormattable
    {
        T Id { get; set; }
    }
}