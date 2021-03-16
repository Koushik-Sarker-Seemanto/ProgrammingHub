using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DomainModels;
using DomainModels.Post;

namespace Services.Abstractions
{
    public interface IPostService
    {
        Task<ResponseModel> GetAllPost(string searchKey, string postType);
        Task<ResponseModel> GetPostByUserId(string userId, bool currentUser);
        Task<ResponseModel> GetPostById(string id);
        Task<ResponseModel> CreatePost(PostModel model, string userName);
        Task<ResponseModel> UpdatePost(string id, PostModel model);
        Task<ResponseModel> DeletePost(string id);
        Task<ResponseModel> Comment(CommentModel model, string userName);
    }
}