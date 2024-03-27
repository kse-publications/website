import axios from 'axios'

const BASE_URL = 'https://publications_api_testing.kse.ua/'

export default class PublicationsApiService {
  static getPublication(id: string) {
    return axios.get(BASE_URL + `publications/${id}`)
  }

  static getPublications() {
    return axios.get(BASE_URL + 'publications')
  }
}
