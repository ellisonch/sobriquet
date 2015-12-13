# Sobriquet
A very simple name generator for C#.
It's a thin veneer on top of [Markov chains](https://en.wikipedia.org/wiki/Markov_chain) optimized for strings.
Because it uses C# strings, it should be suitable for any names (or words) representable in Unicode.

Sobriquet is useful if you have a big list of sample names and you want to generate similar names based on the samples.
It also comes with male and female, first and last names from the 1990 US Census ready to be used.

## License
Sobriquet is licensed using the [MIT license](LICENSE.txt).

## Examples
The main way of using Sobriquet is to provide it with training names, after which it can generate new names in the same style.
```cs
var names = new List<string> {
	"Carlos"
	, "Charlotte"
	, "Marley"
};
var namegen = new Sobriquet.Generator(3, names);
for (int i = 0; i < 4; i++) {
	Console.WriteLine(namegen.Next());
}
```
```
Marlotte
Carlos
Marlos
Charlos
```
The integer argument to the `Generator` constructor is the order of the Markov chain to use, that is, how many characters of history should be used in predicting the next character.
Higher order chains are more realistic, but generally less creative.

To get you started quickly, Sobriquet includes name data from the 1990 US Census.
Here's an example of how to use these builtin names:
```cs
var dg = new Sobriquet.DefaultGenerators(5);
for (int i = 0; i < 4; i++) {
	Console.WriteLine(dg.MaleFirstName.Next() + " " + dg.LastName.Next());
}
```
```
SHANE CULLEN
ROBERT BROOKS
ERIK LOPICCOLOMBOY
MANUEL DESILVA
```
Again, the numerical argument to the constructor is the order of the Markov chain.
