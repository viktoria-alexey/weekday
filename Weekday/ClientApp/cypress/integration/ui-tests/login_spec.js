describe('Connect', () => {
  it('Start page', () => {
    cy.visit('https://localhost:44320/')
	cy.contains('img');
	cy.contains('.navbar-brand');
	cy.contains('.nav-link');
  })
})