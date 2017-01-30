using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private const string IMAGE_NOT_AVAILABLE_PATH = "http://i.annihil.us/u/prod/marvel/i/mg/b/40/image_not_available";

        private static readonly int MAXSIZE = 1500;

        public static async Task UpdateCharacterListAsync(ObservableCollection<Character> characterList)
        {
            var characterDataWrapper = await GetCharacterDataWrapperAsync();
            var characters = characterDataWrapper.data.results;

            foreach (var character in characters) {
                if (character.thumbnail != null && character.thumbnail.path != "" && 
                    character.thumbnail.path != IMAGE_NOT_AVAILABLE_PATH) {

                    character.thumbnail.small = string.Format("{0}/standard_small.{1}", 
                        character.thumbnail.path, character.thumbnail.extension);

                    character.thumbnail.large = string.Format("{0}/detail.{1}",
                        character.thumbnail.path, character.thumbnail.extension);

                    characterList.Add(character);
                }
            }
        }

        public static async Task UpdateComicListAsync(ObservableCollection<Comic> comicList, int characterId)
        {
            var comicDataWrapper = await GetComicDataWrapperAsync(characterId);
            var comics = comicDataWrapper.data.results;

            comicList.Clear();

            foreach (var comic in comics) {
                if (comic.thumbnail != null && comic.thumbnail.path != "" &&
                    comic.thumbnail.path != IMAGE_NOT_AVAILABLE_PATH) {

                    comic.thumbnail.small = string.Format("{0}/portrait_medium.{1}", 
                        comic.thumbnail.path, comic.thumbnail.extension);

                    comicList.Add(comic);
                }
            }
        }

        private static async Task<ComicDataWrapper> GetComicDataWrapperAsync(int characterId)
        {
            // call marvel
            var partialUrl = string.Format(
                "https://gateway.marvel.com:443/v1/public/characters/{0}/comics?limit=10", characterId);

            var jsonMessage = await CallMarvelAsync(partialUrl);

            // create serializer for CharacterDataWrapper
            var serializer = new DataContractJsonSerializer(typeof(ComicDataWrapper));
            // get the content from jsonMessage and store into this stream
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            // convert the bytes in the stream to appropriate classes
            var result = (ComicDataWrapper)serializer.ReadObject(memoryStream);
            System.Diagnostics.Debug.WriteLine("dummy");
            return result;
        }

        private async static Task<CharacterDataWrapper> GetCharacterDataWrapperAsync()
        {
            var randomGenerator = new Random();
            var offset = randomGenerator.Next(MAXSIZE);

            var partialUrl = string.Format(
                "https://gateway.marvel.com:443/v1/public/characters?limit=50&offset={0}", offset);

            var jsonMessage = await CallMarvelAsync(partialUrl);

            // create serializer for CharacterDataWrapper
            var serializer = new DataContractJsonSerializer(typeof(CharacterDataWrapper));
            // get the content from jsonMessage and store into this stream
            var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonMessage));

            // convert the bytes in the stream to appropriate classes
            var result = (CharacterDataWrapper)serializer.ReadObject(memoryStream);
            System.Diagnostics.Debug.WriteLine("dummy");
            return result;
        }

        private static async Task<string> CallMarvelAsync(string partialUrl)
        {
            var timeStamp = DateTime.Now.Ticks.ToString();
            var hashedValue = GetHash(timeStamp);

            var url = string.Format("{0}&apikey={1}&ts={2}&hash={3}", partialUrl, PublicKey, timeStamp, hashedValue);

            HttpClient httpClient = new HttpClient();
            var response = await httpClient.GetAsync(url);
            return await response.Content.ReadAsStringAsync();
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
