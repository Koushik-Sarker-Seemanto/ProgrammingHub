using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Post;
using Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Repository;
using Services.Abstractions;

namespace Services
{
    public class PostService : IPostService
    {
        private readonly IMongoRepository _repository;
        private readonly ILogger<PostService> _logger;
        public PostService(IMongoRepository repository, ILogger<PostService> logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<ResponseModel> GetAllPost(string searchKey, string postType)
        {
            ResponseModel response = new ResponseModel();
            var posts = await this._repository.GetItemsAsync<Post>();
            if (searchKey != null)
            {
                posts = posts.Where(e => e.Title.Contains(searchKey) || e.Description.Contains(searchKey));
            }

            if (postType != null)
            {
                posts = posts.Where(e => e.PostType == postType);
            }
            response.ResponseStatus = ResponseEnum.Ok;
            response.Data = posts.OrderByDescending(e => e.UpdatedAt).ToList();
            return response;
        }

        public async Task<ResponseModel> GetPostById(string id)
        {
            ResponseModel response = new ResponseModel();
            var result = await this._repository.GetItemAsync<Post>(e => e.Id == Guid.Parse(id));
            if (result.Equals(null))
            {
                response.ResponseStatus = ResponseEnum.NotFound;
                response.Response = "Post Not Found";
                return response;
            }

            response.ResponseStatus = ResponseEnum.Ok;
            response.Data = result;
            return response;
        }
        
        public async Task<ResponseModel> GetPostByUserId(string userId, bool currentUser)
        {
            ResponseModel response = new ResponseModel();
            if (string.IsNullOrEmpty(userId))
            {
                response.ResponseStatus = ResponseEnum.NotFound;
                response.Response = "User Id Not Found";
                return response;
            }

            var user = await this._repository.GetItemAsync<User>(e => e.Id == Guid.Parse(userId));
            if (user.Equals(null))
            {
                response.ResponseStatus = ResponseEnum.UserDoesNotExist;
                response.Response = "Invalid User Id";
                return response;
            }
            var result = await this._repository.GetItemsAsync<Post>(e => e.UserId == Guid.Parse(userId));
            if (result.Equals(null))
            {
                response.ResponseStatus = ResponseEnum.NotFound;
                response.Response = "Post Not Found";
                return response;
            }

            ProfileViewModel model = new ProfileViewModel()
            {
                Posts = result.OrderByDescending(e => e.UpdatedAt)?.ToList(),
                UserData = user,
                CurrentUser = currentUser,
            };

            response.ResponseStatus = ResponseEnum.Ok;
            response.Data = model;
            return response;
        }

        [Authorize]
        public async Task<ResponseModel> CreatePost(PostModel model, string userName)
        {
            ResponseModel response = new ResponseModel();
            if(string.IsNullOrEmpty(userName))
            {
                response.ResponseStatus = ResponseEnum.Unauthorized;
                response.Response = "Unauthorized";
                return response;
            }

            var user = await this._repository.GetItemAsync<User>(e => e.Username == userName);
            if (user == null)
            {
                response.ResponseStatus = ResponseEnum.Invalid;
                response.Response = "Invalid User";
                return response;
            }

            if (model == null || string.IsNullOrEmpty(model.Title) || string.IsNullOrEmpty(model.Description))
            {
                response.ResponseStatus = ResponseEnum.Invalid;
                response.Response = "Invalid Input";
                return response;
            }

            Post post = new Post()
            {
                Id = Guid.NewGuid(),
                Title = model.Title,
                Description = model.Description,
                UserId = user.Id,
                Username = user.Username,
                PostType = model.PostType,
                Comments = new List<Comment>(),
            };
            await this._repository.SaveAsync<Post>(post);
            response.ResponseStatus = ResponseEnum.Ok;
            return response;

        }

        public async Task<ResponseModel> UpdatePost(string id, PostModel model)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseModel> DeletePost(string id)
        {
            throw new System.NotImplementedException();
        }

        public async Task<ResponseModel> Comment(CommentModel model, string userName)
        {
            ResponseModel response = new ResponseModel();
            if (string.IsNullOrEmpty(model.PostId) || string.IsNullOrEmpty(model.Comment))
            {
                response.ResponseStatus = ResponseEnum.Invalid;
                response.Response = "Invalid Comment";
                return response;
            }

            var user = await this._repository.GetItemAsync<User>(e => e.Username == userName);
            if (user == null)
            {
                response.ResponseStatus = ResponseEnum.Unauthorized;
                response.Response = "Unauthorized";
                return response;
            }

            var post = await this._repository.GetItemAsync<Post>(e => e.Id == Guid.Parse(model.PostId));
            if (post == null)
            {
                response.ResponseStatus = ResponseEnum.NotFound;
                response.Response = "Post not found";
                return response;
            }

            var comment = new Comment()
            {
                Username = user.Username,
                Description = model.Comment,
                Id = Guid.NewGuid(),
                UserId = user.Id,
                CreatedAt = DateTime.Now,
            };

            if (post.Comments.Equals(null) || !post.Comments.Any())
            {
                post.Comments = new List<Comment>(){comment};
            }
            else
            {
                post.Comments.Add(comment);
            }

            this._logger.LogInformation(JsonConvert.SerializeObject(post));
            await this._repository.UpdateAsync<Post>(e => e.Id == post.Id, post);
            response.ResponseStatus = ResponseEnum.Ok;
            return response;
        }
    }
}