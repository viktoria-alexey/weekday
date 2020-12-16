describe('Connect', () => {
  it('Start page', () => {
    cy.visit('https://localhost:44320/')
	cy.contains('Login').click()
	cy.url().should('include', '/Identity/Account')
  })
})