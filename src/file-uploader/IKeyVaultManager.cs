using System.Threading.Tasks;

namespace FileUploader;
public interface IKeyVaultManager
{
    public Task<string> GetSecret(string secretName);

}