import axios from 'axios'

const BASE_URL = 'https://publications_api_testing.kse.ua/'

axios.interceptors.request.use((config) => {
  config.headers['Content-Type'] = 'application/json'
  config.headers['Access-Control-Allow-Origin'] = '*'
  return config
})

export default class PublicationsApiService {
  static getPublication(id: string) {
    return axios.get(BASE_URL + `publications/${id}`)
  }
}
