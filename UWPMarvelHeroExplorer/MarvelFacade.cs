using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using UWPMarvelHeroExplorer.Models;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage.Streams;

namespace UWPMarvelHeroExplorer {
    public class MarvelFacade
    {

        // read api keys here
        private static string PublicKey = File.ReadAllText("Secret/publickey.txt");
        private static string PrivateKey = File.ReadAllText("Secret/privatekey.txt");

        private static readonly int MAXSIZE = 1500;

        public async static Task<CharacterDataWrapper> GetCharacterListAsync()
        {
            var randomGenerator = new Random();
            var offset = randomGenerator.Next(MAXSIZE);

            // get md5 hash
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hashedValue = GetHash(timeStamp);

            // assemble url
            var url = string.Format(
                "https://gateway.marvel.com:443/v1/public/characters?limit=10&offset={0}&apikey={1}&ts={2}&hash={3}",
                offset, PublicKey, timeStamp, hashedValue);

            // call marvel api
            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            var jsonMessage = await response.Content.ReadAsStringAsync();

            // response -> string -> json -> deserialize

            // create serializer for CharacterDataWrapper
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            // get the content from jsonMessage and store into this stream
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            // convert the bytes in the stream to appropriate classes
            return serializer.ReadObject(memoryStream) as CharacterDataWrapper;
        }

        private static string GetHash(string timeStamp)
        {
            var toBeHashed = timeStamp + PrivateKey + PublicKey;

            // From:
            // http://stackoverflow.com/questions/8299142/how-to-generate-md5-hash-code-for-my-winrt-app-using-c

            var alg = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Md5);
            IBuffer buff = CryptographicBuffer.ConvertStringToBinary(toBeHashed, BinaryStringEncoding.Utf8);
            var hashed = alg.HashData(buff);
            var res = CryptographicBuffer.EncodeToHexString(hashed);
            return res;
        }
    }
}
