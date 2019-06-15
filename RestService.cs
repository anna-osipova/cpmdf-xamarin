using System;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace iosapp
{
    public class RestService
    {

        HttpClient client;

        const String URL = "https://rocky-beach-17461.herokuapp.com";

		public RestService()
		{
			client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Basic YW5uYTp3ZXJlLWtlcHQtZmlndXJl");
			client.MaxResponseContentBufferSize = 256000;
		}

        public async Task<List<TaskItem>> GetTasksAsync() {
            var uri = new Uri(string.Format(URL + "/tasks", string.Empty));
            var response = await client.GetAsync(uri);
            var items = new List<TaskItem>();

            if (response.IsSuccessStatusCode)
			{
                var content = await response.Content.ReadAsStringAsync();
                items = await System.Threading.Tasks.Task.Run(() => JsonConvert.DeserializeObject<List<TaskItem>>(content));
            }
            return items;
        }

        public async Task<TaskItem> PostTaskAsync(TaskItem item) {
            var uri = new Uri(string.Format(URL + "/tasks", string.Empty));

            var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PostAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var responseItem = await System.Threading.Tasks.Task.Run(() => JsonConvert.DeserializeObject<TaskItem>(content));
                return responseItem;
            } else {
                return null;
            }
        }

		public async Task<TaskItem> UpdateTaskAsync(TaskItem item)
		{
            var uri = new Uri(string.Format(URL + "/tasks/" + item.id, string.Empty));

			var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.PutAsync(uri, new StringContent(serializedItem, Encoding.UTF8, "application/json"));
			if (response.IsSuccessStatusCode)
			{
				var content = await response.Content.ReadAsStringAsync();
				var responseItem = await System.Threading.Tasks.Task.Run(() => JsonConvert.DeserializeObject<TaskItem>(content));
				return responseItem;
			}
			else
			{
				return null;
			}
		}

        public async Task<Boolean> DeleteTaskAsync(TaskItem item)
		{
            var uri = new Uri(string.Format(URL + "/tasks/" + item.id, string.Empty));

			var serializedItem = JsonConvert.SerializeObject(item);

            var response = await client.DeleteAsync(uri);
			if (response.IsSuccessStatusCode)
			{
				return true;
			}
			else
			{
                return false;
			}
		}
    }
}
