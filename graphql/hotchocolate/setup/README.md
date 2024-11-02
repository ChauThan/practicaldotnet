# Setting up `graphql` by using Hot Chocolate
This is a straightforward guide on how to set up Hot Chocolate. Here are some important notes:
- Use the code-first approach
- SDL
```sdl
type Query {
  authors: [Author] @cost(weight: "10")
  author(id: Int!): Author @cost(weight: "10")
  books: [Book] @cost(weight: "10")
  book(id: Int!): Book @cost(weight: "10")
}
```