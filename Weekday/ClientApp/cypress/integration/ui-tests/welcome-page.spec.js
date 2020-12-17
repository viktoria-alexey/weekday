describe('Start page', () => {
  it('check login link', () => {
    cy.visit('https://localhost:5001/')
    cy.contains('Login').click()
    cy.url().should('include', '/Identity/Account')
  })

  it('check markup', () => {
    cy.visit('https://localhost:5001/')
    cy.get('app-home').find('img');
    cy.get('body').find('app-nav-menu');
  })
})