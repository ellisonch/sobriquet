# Sobriquet
A very simple name generator.
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

Sobriquet includes data from the 1990 US Census to get you started quickly.
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