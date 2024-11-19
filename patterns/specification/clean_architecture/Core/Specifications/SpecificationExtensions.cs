namespace App.Core.Specifications;

public static class SpecificationExtensions
{
    public static ISpecification<T> And<T>(this ISpecification<T> left, ISpecification<T> right) => new AndSpecification<T>(left, right);
    public static ISpecification<T> Or<T>(this ISpecification<T> left, ISpecification<T> right) => new OrSpecification<T>(left, right);
    public static ISpecification<T> Not<T>(this ISpecification<T> specification) => new NotSpecification<T>(specification);
    public static bool IsSatisfiedBy<T>(this ISpecification<T> specification, T item) => specification.ToExpression().Compile()(item);
}