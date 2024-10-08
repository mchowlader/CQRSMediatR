﻿namespace CQRSMediator.Models;

public class PaginationModel
{
    const int maxPageSize = 30;
    public int PageNumber { get; set; } = 1;
    private int _pageSize = 30;
    public int PageSize
    {
        get
        {
            return _pageSize;
        }
        set
        {
            _pageSize = (value > maxPageSize) ? maxPageSize : value;
        }
    }
}