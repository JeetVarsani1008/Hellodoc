using LibraryManagement.BLL.Interface;
using LibraryManagement.DAL.DataContext;
using LibraryManagement.DAL.DataModels;
using LibraryManagement.DAL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryManagement.BLL.Repository
{
    public class LibraryDashboard : ILibraryDashboard
    {
        private readonly ApplicationDbContext _context;

        public LibraryDashboard(ApplicationDbContext context)
        {
            _context = context;
        }

        #region getLibraryDashboardData
        public List<LibraryDashboardData> getLibraryDashboardData(string searchdata)
        {
            var list = _context.Books.ToList();

            if(searchdata != null)
            {
                searchdata = searchdata.ToLower();
                list = list.Where(x => x.BookName.ToLower().Contains(searchdata) || x.Author.ToLower().Contains(searchdata) || x.BorrowerName.ToLower().Contains(searchdata) || x.City.ToLower().Contains(searchdata) || x.Genere.ToLower().Contains(searchdata)).ToList();
            }

            var data = list.Select(x => new LibraryDashboardData
            {
                BookId = x.Id,
                BookName = x.BookName,
                Author = x.Author,
                BorrowerName = x.BorrowerName,
                DateOfIssue = x.DateOfIssue,
                City = x.City,
                Genere = x.Genere,
            }).ToList();
            return data;
        }
        #endregion

        #region addLibraryBookData
        public bool addLibraryBookData(LibraryAddVm model)
        {
            //it check id borrower name is present in database then it will not add entry in borrow table and take id from borrow table
            var checkBorrower = _context.Books.Any(x => x.BorrowerName.ToLower() == model.BorrowerName.ToLower());
            if(checkBorrower)
            {
                var takeBorrowerId = _context.Books.FirstOrDefault(x => x.BorrowerName.ToLower() == model.BorrowerName.ToLower()).BorrowerId;

                Book save = new Book
                {
                    BookName = model.BookName,
                    Author = model.Author,
                    BorrowerId = takeBorrowerId,
                    BorrowerName = model.BorrowerName,
                    DateOfIssue = model.DateOfIssue,
                    City = model.City,
                    Genere = model.Genere,
                };
                _context.Books.Add(save);
                _context.SaveChanges();
            }
            else
            {
                Borrower borrower = new Borrower();
                borrower.City = model.City;
                _context.Borrowers.Add(borrower);
                _context.SaveChanges();

                Book book = new Book
                {
                    BookName = model.BookName,
                    Author = model.Author,  
                    BorrowerId = borrower.BorrowerId,
                    BorrowerName = model.BorrowerName,
                    DateOfIssue = model.DateOfIssue,    
                    City = model.City,
                    Genere = model.Genere,
                };
                _context.Books.Add(book);
                _context.SaveChanges();
            }

            return true;
        }
        #endregion

        #region getEditBookData
        public LibraryAddVm getEditBookData(int bookId)
        {
            var data = _context.Books.FirstOrDefault(x => x.Id == bookId);
            try
            {
                LibraryAddVm lib = new LibraryAddVm();
                lib.BookId = data.Id;
                lib.BookName = data.BookName;
                lib.Author = data.Author;
                lib.BorrowerName = data.BorrowerName;
                lib.DateOfIssue = data.DateOfIssue;
                lib.City = data.City;
                lib.Genere = data.Genere;
                return lib;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region editBookDataPost
        public bool editBookDataPost(LibraryAddVm model)
        {
            //here, it will check borrower name if it already exist then asign borrower id to book record
            var checkBorrower = _context.Books.Any(x => x.BorrowerName.ToLower() == model.BorrowerName.ToLower());
            if (checkBorrower)
            {
                var takeBorrowerId = _context.Books.FirstOrDefault(x => x.BorrowerName.ToLower() == model.BorrowerName.ToLower()).BorrowerId;
                var datatochangeid = _context.Books.FirstOrDefault(x => x.Id == model.BookId);
                datatochangeid.BorrowerId = takeBorrowerId;
                _context.SaveChanges();
            }

            var data = _context.Books.FirstOrDefault(x => x.Id == model.BookId);
            data.BookName = model.BookName;
            data.Author = model.Author; 
            data.BorrowerName = model.BorrowerName; 
            data.DateOfIssue = model.DateOfIssue;
            data.City = model.City;
            data.Genere = model.Genere;

            _context.SaveChanges();
            return true;
        }
        #endregion

        #region bookDeleteData
        public void bookDeleteData(int bookId)
        {
            var data = _context.Books.FirstOrDefault(x => x.Id == bookId);
            _context.Books.Remove(data);
            _context.SaveChanges();
        }
        #endregion
    }
}
