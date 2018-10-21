﻿using Microsoft.EntityFrameworkCore.Storage;
using Mix.Cms.Lib.Models.Cms;
using Mix.Common.Helper;
using Mix.Domain.Core.ViewModels;
using Mix.Domain.Data.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mix.Cms.Lib.ViewModels.MixPortalPagePortalPages
{
    public class UpdateViewModel
       : ViewModelBase<MixCmsContext, MixPortalPageNavigation, UpdateViewModel>
    {
        public UpdateViewModel(MixPortalPageNavigation model, MixCmsContext _context = null, IDbContextTransaction _transaction = null)
            : base(model, _context, _transaction)
        {
        }

        public UpdateViewModel() : base()
        {
        }


        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("parentId")]
        public int ParentId { get; set; }


        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }


        #region Views

        [JsonProperty("isActived")]
        public bool IsActived { get; set; }

        [JsonProperty("page")]
        public MixPortalPages.UpdateRolePermissionViewModel Page { get; set; }

        [JsonProperty("parent")]
        public MixPortalPages.ReadViewModel ParentPage { get; set; }

        #endregion Views

        #region overrides
        public override async Task<RepositoryResponse<bool>> SaveSubModelsAsync(MixPortalPageNavigation parent, MixCmsContext _context, IDbContextTransaction _transaction)
        {
            var result = await Page.SaveModelAsync(false, _context, _transaction);
            return new RepositoryResponse<bool>()
            {
                IsSucceed = result.IsSucceed,
                Data = result.IsSucceed,
                Errors = result.Errors,
                Exception = result.Exception
            };
        }
        public override void ExpandView(MixCmsContext _context = null, IDbContextTransaction _transaction = null)
        {
            var getCategory = MixPortalPages.UpdateRolePermissionViewModel.Repository.GetSingleModel(p => p.Id == Id
               
            );
            if (getCategory.IsSucceed)
            {
                Page = getCategory.Data;
            }
            var getParent = MixPortalPages.ReadViewModel.Repository.GetSingleModel(p => p.Id == ParentId
                , _context: _context, _transaction: _transaction
            );
            if (getParent.IsSucceed)
            {
                ParentPage = getParent.Data;
            }
        }

        #endregion overrides

        #region Expands

        public static async System.Threading.Tasks.Task<RepositoryResponse<List<UpdateViewModel>>> UpdateInfosAsync(List<MixPortalPagePortalPages.UpdateViewModel> cates)
        {
            MixCmsContext context = new MixCmsContext();
            var transaction = context.Database.BeginTransaction();
            var result = new RepositoryResponse<List<UpdateViewModel>>();
            try
            {

                foreach (var item in cates)
                {
                    var saveResult = await item.SaveModelAsync(false, context, transaction);
                    result.IsSucceed = saveResult.IsSucceed;
                    if (!result.IsSucceed)
                    {
                        result.Errors.AddRange(saveResult.Errors);
                        result.Exception = saveResult.Exception;
                        break;
                    }
                }
                UnitOfWorkHelper<MixCmsContext>.HandleTransaction(result.IsSucceed, true, transaction);
                return result;
            }
            catch (Exception ex) // TODO: Add more specific exeption types instead of Exception only
            {
                UnitOfWorkHelper<MixCmsContext>.HandleException<UpdateViewModel>(ex, true, transaction);
                return new RepositoryResponse<List<UpdateViewModel>>()
                {
                    IsSucceed = false,
                    Data = null,
                    Exception = ex
                };
            }
            finally
            {
                //if current Context is Root
                transaction.Dispose();
                context.Dispose();
            }
        }
        #endregion
    }
}
