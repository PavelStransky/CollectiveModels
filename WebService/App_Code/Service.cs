using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;

using PavelStransky.Math;

[WebService(Namespace = "http://is.gabrielos.cz/")]
[WebServiceBinding(ConformsTo = WsiProfiles.None)]
public class Service : System.Web.Services.WebService
{
    public Service () {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }

    [WebMethod(Description = "Pokus hokus")]
    public double[] HelloWorld(int x, int y) {
        return new double[x * y];
    }
   
}
