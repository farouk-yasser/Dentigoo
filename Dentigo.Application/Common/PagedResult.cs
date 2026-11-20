using System;
using System.Collections.Generic;

namespace Dentigo.Application.Common
{
    public class PagedResult<T>
    {
        private IReadOnlyList<T> _items = Array.Empty<T>();
        private int _pageNumber = 1;
        private int _pageSize = 10;
        private int _totalCount = 0;

        public IReadOnlyList<T> Items
        {
            get => _items;
            set => _items = value ?? Array.Empty<T>();
        }

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber = value < 1 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 10 : value;
        }

        public int TotalCount
        {
            get => _totalCount;
            set => _totalCount = value < 0 ? 0 : value;
        }

        public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
        public bool HasPreviousPage => PageNumber > 1 && TotalPages > 0;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedResult()
        {
        }

        public PagedResult(IReadOnlyList<T>? items, int totalCount, int pageNumber, int pageSize)
        {
            Items = items ?? Array.Empty<T>();
            TotalCount = totalCount;
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
