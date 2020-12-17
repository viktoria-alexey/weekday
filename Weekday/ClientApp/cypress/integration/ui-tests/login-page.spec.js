describe('Login page', () => {
    it('check login form', () => {
        cy.visit('https://localhost:5001/');
        cy.get('ul li a').click();
        cy.get('#account > div.col-md-6 > div:nth-child(1) > label').contains('User name');
        cy.get('#account > div.col-md-6 > div:nth-child(2) > label').contains('Password');
    })
});