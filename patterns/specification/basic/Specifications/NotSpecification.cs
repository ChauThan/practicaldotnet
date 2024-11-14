namespace App.Specifications;

public class NotSpecification<T>(ISpecification<T> spec) : ISpecification<T>
{
    public bool IsSatisfiedBy(T item) => !spec.IsSatisfiedBy(item);
}