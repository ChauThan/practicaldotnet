namespace App.Specifications;

public class OrSpecification<T>(
    ISpecification<T> left, 
    ISpecification<T> right) 
    : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => left.IsSatisfiedBy(item) || right.IsSatisfiedBy(item);
}