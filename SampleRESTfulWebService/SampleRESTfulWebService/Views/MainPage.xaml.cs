using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SampleRESTfulWebService.Data;
using Xamarin.Forms;

namespace SampleRESTfulWebService.Views
{
	public partial class MainPage : ContentPage
	{
		// cria lista de livros
		readonly IList<Book> books = new ObservableCollection<Book>();
		// cria o Data.BookManager para gerenciar a comunicação com a WebAPI
		readonly BookManager manager = new BookManager();

		public MainPage()
		{	
			// BindingContext da tela é da lista de livros
			BindingContext = books;
			InitializeComponent();
		}

		// Método para carregar a lista 
		// Utilizando o ActivityIndicator para reportar ao usuário 
		// o processamento do comando
		async void OnRefresh(object sender, EventArgs e)
		{			 
			this.IsBusy = true; // Ativar o ActivityIndicator

			try
			{
				var bookCollection = await manager.GetAll();

				foreach (Book book in bookCollection)
				{
					if (books.All(b => b.ISBN != book.ISBN))
						books.Add(book);
				}
			}
	
			finally
			{
				this.IsBusy = false; // Desativar o ActivityIndicator
			}
		}

		// Evento do botão da ToolBar para adicionar um novo livro, 
		// chamando a page AddEditBookPage
		async void OnAddNewBook(object sender, EventArgs e)
		{
			await Navigation.PushModalAsync(
				new AddEditBookPage(manager, books));
		}

		// Evento ItemTapped da ListView chamando a page AddEditBookPage
		async void OnEditBook(object sender, ItemTappedEventArgs e)
		{
			await Navigation.PushModalAsync(
				new AddEditBookPage(manager, books, (Book)e.Item));
		}

		// Evento Clicked do item da lista passando o item para deletar
		async void OnDeleteBook(object sender, EventArgs e)
		{
			MenuItem item = (MenuItem)sender; // converte o item selecionado em MenuItem
			Book book = item.CommandParameter as Book; // gera um livro a partir do item selecionado 
			if (book != null)
			{
				if (await this.DisplayAlert("Delete Book?",
					"Are you sure you want to delete the book '"
						+ book.Title + "'?", "Yes", "Cancel") == true)
				{
					this.IsBusy = true; // Ativar o ActivityIndicator
					try
					{
						await manager.Delete(book.ISBN); // Passa o livro para deletar
						books.Remove(book); // remove o livro da lista da tela. 
					}
					finally
					{
						this.IsBusy = false; // Desativar o ActivityIndicator
					}

				}
			}
		}
	}
}
