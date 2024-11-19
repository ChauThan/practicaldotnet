using System.Linq.Expressions;
using App.Core;
using App.Core.Specifications;
using AutoMapper;

namespace App.Infrastructure;

public class SpecificationConverter
{
    private readonly IMapper _mapper;

    public SpecificationConverter(IMapper mapper)
    {
        _mapper = mapper;
    }

    public ISpecification<ProductEntity> Convert(ISpecification<Product> specification)
    {
        var productExpression = specification.ToExpression();
        return new ConvertedSpecification(_mapper, productExpression);
    }

    private class ConvertedSpecification : ISpecification<ProductEntity>
    {
        private readonly IMapper _mapper;
        private readonly Expression<Func<Product, bool>> _productExpression;

        public ConvertedSpecification(IMapper mapper, Expression<Func<Product, bool>> productExpression)
        {
            _mapper = mapper;
            _productExpression = productExpression;
        }

        public Expression<Func<ProductEntity, bool>> ToExpression()
        {
            var parameter = Expression.Parameter(typeof(ProductEntity), "pe");
            var mappedProduct = _mapper.Map<Product>(Expression.Lambda<Func<ProductEntity>>(parameter).Compile()());

            var body = ReplaceParameter(_productExpression.Body, _productExpression.Parameters[0], Expression.Constant(mappedProduct));
            return Expression.Lambda<Func<ProductEntity, bool>>(body, parameter);
        }

        private static Expression ReplaceParameter(Expression body, ParameterExpression toReplace, Expression newExpression)
        {
            return new ParameterReplacer(toReplace, newExpression).Visit(body);
        }

        private class ParameterReplacer : ExpressionVisitor
        {
            private readonly ParameterExpression _toReplace;
            private readonly Expression _newExpression;

            public ParameterReplacer(ParameterExpression toReplace, Expression newExpression)
            {
                _toReplace = toReplace;
                _newExpression = newExpression;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                return node == _toReplace ? _newExpression : base.VisitParameter(node);
            }
        }
    }
}
