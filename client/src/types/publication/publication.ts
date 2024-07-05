interface Author {
  name: string
  profilelink: string
  slug: string
  views: number
}

interface Publisher {
  name: string
  slug: string
  views: number
}

export interface Publication {
  id: number
  title: string
  type: string
  language: string
  year: number
  link: string
  keywords: string[]
  abstracttext: string
  authors: Author[]
  publisher: Publisher
  lastmodified: string
  slug: string
  views: number
}
