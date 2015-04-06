﻿using System;
using System.Linq.Expressions;

namespace LinqToDB.Linq.Builder
{
	class QueryExpression<T> : Expression
	{
		public QueryExpression(IExpressionBuilder expressionBuilder)
		{
			AddBuilder(_first = expressionBuilder);
		}

		Type               _type;
		IExpressionBuilder _first;
		IExpressionBuilder _last;

		public override Type           Type      { get { return _type; } }
		public override bool           CanReduce { get { return true;  } }
		public override ExpressionType NodeType  { get { return ExpressionType.Extension; } }

		public override Expression Reduce()
		{
			var expr = _last.BuildQuery<T>();

			_type = expr.Type;

			return expr;
		}

		public void BuildQuery(Query<T> query)
		{
			_last.BuildQuery(query);
		}

		public QueryExpression<T> AddBuilder(IExpressionBuilder expressionBuilder)
		{
			for (var builder = expressionBuilder; builder != null; builder = builder.Next)
			{
				if (_last != null)
				{
					_last.Next = expressionBuilder;
					expressionBuilder.Prev = _last;
				}

				_type = expressionBuilder.Type;
				_last = expressionBuilder;
			}

			return this;
		}
	}
}