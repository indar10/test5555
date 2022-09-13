namespace IDMSCommonService
{
    public partial class IDMSIQServiceClient : System.ServiceModel.ClientBase<IDMSCommonService.IIDMSIQService>, IDMSCommonService.IIDMSIQService
    {
        public IDMSIQServiceClient(string strVal) :
                base(IDMSIQServiceClient.GetDefaultBinding(), IDMSIQServiceClient.GetDefaultEndpointAddress(strVal))
        {
            this.Endpoint.Name = EndpointConfiguration.BasicHttpBinding_IIDMSIQService.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        private static System.ServiceModel.EndpointAddress GetDefaultEndpointAddress(string strVal)
        {
            return IDMSIQServiceClient.GetEndpointAddress(EndpointConfiguration.BasicHttpBinding_IIDMSIQService, strVal);
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration, string strVal)
        {
            if ((endpointConfiguration == EndpointConfiguration.BasicHttpBinding_IIDMSIQService))
            {
                return new System.ServiceModel.EndpointAddress(strVal);
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }
    }

}
