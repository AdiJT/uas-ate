namespace UASATE.Core.Abstracts;

public interface IEncoding
{
    Vector Encode(Vector vector);
    Vector Decode(Vector vector);

    Vector Constraint(Vector vector);
}
