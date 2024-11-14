namespace App.Specifications;

public interface ISpecification<in T>
{
    bool IsSatisfiedBy(T item);
}