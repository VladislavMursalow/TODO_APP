using Buisness;
using Buisness.Models;
using Buisness.Repositories.Interfaces;
using GraphQL;
using GraphQL.Types;
using GraphQL_API.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace GraphQL_API
{
    public class TodoAppQuery : ObjectGraphType
    {
        DataSource dataSource;
        ITodoRepository todoRepository;
        ICategoriesRerository categoriesRerository;
        public TodoAppQuery(RepositoryResolver repositoryReslover, IHttpContextAccessor httpContextAccesor)
        {
            string dataSourceStr = httpContextAccesor.HttpContext.Request.Headers["dataSource"];
            if (Enum.TryParse(dataSourceStr, out dataSource))
            {
                todoRepository = repositoryReslover.ResolveTodoRepository(dataSource);
                categoriesRerository = repositoryReslover.ResolveCategoryRepository(dataSource);
            }
            else
            {
                todoRepository = repositoryReslover.ResolveTodoRepository(DataSource.MsSql);
                categoriesRerository = repositoryReslover.ResolveCategoryRepository(DataSource.MsSql);
            }

            //Todo queries
            Field<ListGraphType<TodoType>>()
                .Name("Todos")
                .Argument<NonNullGraphType<IntGraphType>, int>("Page", "Numper of page")
                .Argument<NonNullGraphType<IntGraphType>, int>("Count", "count of items")
                .ResolveAsync(async context =>
                {
                    int page = context.GetArgument<int>("Page");
                    int count = context.GetArgument<int>("Count");
                    return await todoRepository.GetTodosByPaginationAsync(page, count);
                });

            Field<IntGraphType>()
                .Name("getCount")
                .ResolveAsync(async context =>
                {
                    var todos = await todoRepository.GetTodosAsync();
                    return (todos as List<TodoModel>).Count;
                });

            Field<TodoType, TodoModel>()
               .Name("Todo")
               .Argument<NonNullGraphType<IntGraphType>, int>("Id", "Argument for get todo by Id")
               .ResolveAsync(async context =>
               {
                   int id = context.GetArgument<int>("Id");
                   var todo = await todoRepository.GetTodoByIdAsync(id);
                   return todo;
               });

            Field<ListGraphType<TodoType>, IEnumerable<TodoModel>>()
              .Name("TodoByCategory")
              .Argument<NonNullGraphType<IntGraphType>, int>("CategoryId", "Argument for get todo by category id")
              .ResolveAsync(async context =>
              {
                  int id = context.GetArgument<int>("CategoryId");
                  var todos = await todoRepository.GetTodoByCategoryAsync(id);
                  return todos;
              });

            //Categories queries
            Field<ListGraphType<CategoryType>>()
                .Name("Categories")
                .ResolveAsync(async context => await categoriesRerository.GetCategoriesAsync());

            Field<CategoryType, CategoryModel>()
               .Name("Category")
               .Argument<NonNullGraphType<IntGraphType>, int>("Id", "Argument for get category by Id")
               .ResolveAsync(async context =>
               {
                   int id = context.GetArgument<int>("Id");
                   var category = await categoriesRerository.GetCategoryByIdAsync(id);
                   return category;
               });
        }
    }
}
