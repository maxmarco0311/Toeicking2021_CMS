using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021
{   
    // 全域物件轉換設定
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            //使用CreateMap<AddCharacterDto, Character>();建立物件轉換關係
            //泛型1為source物件型別，泛型2為destination物件型別
        }
    }
}
