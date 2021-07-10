using AutoMapper;
using Mango.Services.CouponAPI.DbContexts;
using Mango.Services.CouponAPI.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mango.Services.CouponAPI.Repository
{
    public class CouponRepository : ICouponRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CouponRepository(ApplicationDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CouponDto> GetCouponByCode(string code)
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(x => x.CouponCode == code);
            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
