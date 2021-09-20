using PagedList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArkaApp.Models.ViewModels
{
    #region ManagementOnly
    public class UserSearchViewModel
    {
        [Display(Name = "نام کاربری")]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "نام و نام خانوادگی")]
        [DisplayName("نام و نام خانوادگی")]
        public string FullName { get; set; }

        [Display(Name = "آدرس پست الکترونیک")]
        [DisplayName("آدرس پست الکترونیک")]
        public string EmailAddress { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public int IsEnabled { get; set; }

        [Display(Name = "نقش کاربر")]
        [DisplayName("نقش کاربر")]
        public int UserRoleID { get; set; }

        [Display(Name = "ثبت شده توسط")]
        [DisplayName("ثبت شده توسط")]
        public int CreatedBy { get; set; }
        public int? Page { get; set; }
        public IPagedList<ViewModels.UserViewModel> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Creators { get; set; }
        public SelectList UserRoles { get; set; }
    }
    public class UserRoleSearchViewModel
    {
        [Display(Name = "عنوان")]
        [DisplayName("عنوان")]
        public string Title { get; set; }

        [Display(Name = "عنوان فارسی")]
        [DisplayName("عنوان فارسی")]
        public string TitleFa { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public int IsEnabled { get; set; }

        [Display(Name = "ثبت شده توسط")]
        [DisplayName("ثبت شده توسط")]
        public int CreatedBy { get; set; }

        public int? Page { get; set; }
        public IPagedList<UserRoleViewModel> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Creators { get; set; }
    }
    public class AccountSearchViewModel
    {
        [Display(Name = "نام کاربری")]
        [DisplayName("نام کاربری")]
        public string UserName { get; set; }

        [Display(Name = "وضعیت")]
        [DisplayName("وضعیت")]
        public int IsEnabled { get; set; }

        [Display(Name = "کاربر")]
        [DisplayName("کاربر")]
        public int UserID { get; set; }

        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.Account> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Users { get; set; }
    }
    #endregion

    public class FollowerSearchViewModel
    {
        [Display(Name = "حساب")]
        [DisplayName("حساب")]
        public int Id { get; set; }

        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.FollowerLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Accounts { get; set; }
    }
    public class LikeSearchViewModel
    {
        [Display(Name = "حساب")]
        [DisplayName("حساب")]
        public int Id { get; set; }

        public int? Page { get; set; }
        public IPagedList<EntityModels.LikeLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Accounts { get; set; }
    }
    public class CommentSearchViewModel
    {
        [Display(Name = "حساب")]
        [DisplayName("حساب")]
        public int Id { get; set; }

        public int? Page { get; set; }
        public IPagedList<EntityModels.CommentLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Accounts { get; set; }
    }
    public class ViewSearchViewModel
    {
        [Display(Name = "حساب")]
        [DisplayName("حساب")]
        public int Id { get; set; }

        public int? Page { get; set; }
        public IPagedList<EntityModels.ViewLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
        public SelectList Accounts { get; set; }
    }

    public class PostSearchViewModel
    {
        [Display(Name = "کپشن")]
        [DisplayName("کپشن")]
        public string Caption { get; set; }
        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.Post> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }
    public class ContentSearchViewModel
    {
        [Display(Name = "متن")]
        [DisplayName("متن")]
        public string Description { get; set; }
        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.Content> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }

    public class ChargeSearchViewModel
    {
        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.WalletChargeLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }
    public class PaymentSearchViewModel
    {
        public int? Page { get; set; }
        public IPagedList<Models.EntityModels.WalletPaymentLog> SearchResults { get; set; }
        public string SearchButton { get; set; }
    }

}