namespace Mobi.Service.AccessControls
{
    public interface IAccessControlService
    {
        bool HasAccess(string screenAuthority);
    }
}
