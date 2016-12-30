using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SampleRESTfulWebService.Data
{
	public class BookManager
	{
		const string Url = "http://xam150.azurewebsites.net/api/books/";

		private string authorizationKey;

		// HttpClient usa Task e asynchornous APIs to realizar operaçoes I/O
		private async Task<HttpClient> GetClient()
		{
			HttpClient client = new HttpClient();
			if (string.IsNullOrEmpty(authorizationKey))
			{
				authorizationKey = await client.GetStringAsync(Url + "login");
				authorizationKey = JsonConvert.DeserializeObject<string>(authorizationKey);
			}

			client.DefaultRequestHeaders.Add("Authorization", authorizationKey);
			client.DefaultRequestHeaders.Add("Accept", "application/json");
			return client;
		}

		public async Task<IEnumerable<Book>> GetAll()
		{
			// GET para recuperar livros
			HttpClient client = await GetClient();
			string result = await client.GetStringAsync(Url);
			return JsonConvert.DeserializeObject<IEnumerable<Book>>(result);
		}

		public async Task<Book> Add(string title, string author, string genre)
		{
			// POST para adicionar um livro
			Book book = new Book()
			{
				Title = title,
				Authors = new List<string>(new[] { author }),
				ISBN = string.Empty,
				Genre = genre,
				PublishDate = DateTime.Now.Date,
			};

			HttpClient client = await GetClient();
			var response = await client.PostAsync(Url,
				new StringContent(
					JsonConvert.SerializeObject(book),
					Encoding.UTF8, "application/json"));

			return JsonConvert.DeserializeObject<Book>(
				await response.Content.ReadAsStringAsync());
		}

		public async Task Update(Book book)
		{
			// PUT para atualizar um livro
			HttpClient client = await GetClient();
			await client.PutAsync(Url + "/" + book.ISBN,
				new StringContent(
					JsonConvert.SerializeObject(book),
					Encoding.UTF8, "application/json"));
		}

		public async Task Delete(string isbn)
		{
			// DELETE para apagar um livro
			HttpClient client = await GetClient();
			await client.DeleteAsync(Url + "/" + isbn);
		}
	}
}
