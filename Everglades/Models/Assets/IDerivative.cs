using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Everglades.Models
{
    public interface IDerivative : IAsset
    {
        String getType();
        List<Param> getParam();
        void setParam(List<Param> param);
    }
}