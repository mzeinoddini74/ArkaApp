using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace ArkaApp.Helper.Api
{
    public class ApiHelper
    {
        string Baseurl = "http://88.80.148.182:5020/";
        public HttpResponseMessage Get(string Token, int PageNumber, int PageSize, string VoucherNumber, string FiscalYear, string StoreCode)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("Token", Token);

                return client.GetAsync("get?PageNumber=" + PageNumber +
                    "&PageSize=" + PageSize +
                    "&VoucherNumber=" + VoucherNumber +
                    "&FiscalYear=" + FiscalYear +
                    "&StoreCode=" + StoreCode).Result;
            }
        }

        public HttpResponseMessage IncreaseLike(string PostId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("Token", Token);

                //var parameters = new Dictionary<string, object> { { "Username", VoucherNumber }, { "StoreCode", StoreCode } };
                //string json = JsonConvert.SerializeObject(parameters);

                return client.PostAsync("IncreaseLike?postId=" + PostId, null).Result;
            }
        }

        public HttpResponseMessage IncreaseComment(string PostId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("Token", Token);

                //var parameters = new Dictionary<string, object> { { "Username", VoucherNumber }, { "StoreCode", StoreCode } };
                //string json = JsonConvert.SerializeObject(parameters);

                return client.PostAsync("IncreaseComment?postId=" + PostId, null).Result;
            }
        }

        public HttpResponseMessage IncreaseView(string PostId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("Token", Token);

                //var parameters = new Dictionary<string, object> { { "Username", VoucherNumber }, { "StoreCode", StoreCode } };
                //string json = JsonConvert.SerializeObject(parameters);

                return client.PostAsync("IncreaseView?postId=" + PostId, null).Result;
            }
        }

        public HttpResponseMessage IncreaseFollower(string Username)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                //client.DefaultRequestHeaders.Clear();
                //client.DefaultRequestHeaders.Add("Token", Token);

                //var parameters = new Dictionary<string, object> { { "Username", VoucherNumber }, { "StoreCode", StoreCode } };
                //string json = JsonConvert.SerializeObject(parameters);

                return client.PostAsync("IncreaseFollower?username=" + Username, null).Result;
            }
        }

    }
}