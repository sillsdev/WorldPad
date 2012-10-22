﻿using System.Collections.Generic;

namespace SIL.HermitCrab
{
	/// <summary>
	/// This class represents a morphological rule which combines two words in to one word.
	/// </summary>
	public class CompoundingRule : MorphologicalRule
	{
		/// <summary>
		/// This represents a compounding subrule.
		/// </summary>
		public class Subrule : Allomorph
		{
			AlphaVariables m_alphaVars;
			MorphologicalTransform m_transform;
			PhoneticPattern m_headLhsTemp;
			PhoneticPattern m_nonHeadLhsTemp;
			int m_firstNonHeadPartition;

			MPRFeatureSet m_excludedMPRFeatures = null;
			MPRFeatureSet m_requiredMPRFeatures = null;
			MPRFeatureSet m_outputMPRFeatures = null;

			/// <summary>
			/// Initializes a new instance of the <see cref="Subrule"/> class.
			/// </summary>
			/// <param name="id">The id.</param>
			/// <param name="desc">The description.</param>
			/// <param name="morpher">The morpher.</param>
			/// <param name="headLhs">The head LHS.</param>
			/// <param name="nonHeadLhs">The non-head LHS.</param>
			/// <param name="rhs">The RHS.</param>
			/// <param name="alphaVars">The alpha variables.</param>
			public Subrule(string id, string desc, Morpher morpher, IEnumerable<PhoneticPattern> headLhs,
				IEnumerable<PhoneticPattern> nonHeadLhs, IEnumerable<MorphologicalOutput> rhs, AlphaVariables alphaVars)
				: base (id, desc, morpher)
			{
				m_alphaVars = alphaVars;

				List<PhoneticPattern> lhs = new List<PhoneticPattern>();
				lhs.AddRange(headLhs);
				lhs.AddRange(nonHeadLhs);

				m_transform = new MorphologicalTransform(lhs, rhs, MorphologicalTransform.RedupMorphType.IMPLICIT);

				// the LHS template is generated by simply concatenating all of the
				// LHS partitions; it matches the entire word, so we check for both the
				// left and right margins.
				m_headLhsTemp = new PhoneticPattern();
				m_headLhsTemp.Add(new MarginContext(Direction.LEFT));
				int partition = 0;
				foreach (PhoneticPattern pat in headLhs)
					m_headLhsTemp.AddPartition(pat, partition++);
				m_headLhsTemp.Add(new MarginContext(Direction.RIGHT));

				m_firstNonHeadPartition = partition;
				m_nonHeadLhsTemp = new PhoneticPattern();
				m_nonHeadLhsTemp.Add(new MarginContext(Direction.LEFT));
				foreach (PhoneticPattern pat in nonHeadLhs)
					m_nonHeadLhsTemp.AddPartition(pat, partition++);
				m_nonHeadLhsTemp.Add(new MarginContext(Direction.RIGHT));
			}

			/// <summary>
			/// Gets or sets the excluded MPR features.
			/// </summary>
			/// <value>The excluded MPR features.</value>
			public MPRFeatureSet ExcludedMPRFeatures
			{
				get
				{
					return m_excludedMPRFeatures;
				}

				set
				{
					m_excludedMPRFeatures = value;
				}
			}

			/// <summary>
			/// Gets or sets the required MPR features.
			/// </summary>
			/// <value>The required MPR features.</value>
			public MPRFeatureSet RequiredMPRFeatures
			{
				get
				{
					return m_requiredMPRFeatures;
				}

				set
				{
					m_requiredMPRFeatures = value;
				}
			}

			/// <summary>
			/// Gets or sets the output MPR features.
			/// </summary>
			/// <value>The output MPR features.</value>
			public MPRFeatureSet OutputMPRFeatures
			{
				get
				{
					return m_outputMPRFeatures;
				}

				set
				{
					m_outputMPRFeatures = value;
				}
			}

			/// <summary>
			/// Unapplies this subrule to the input word analysis.
			/// </summary>
			/// <param name="input">The input word analysis.</param>
			/// <param name="output">The output word analyses.</param>
			/// <returns><c>true</c> if the subrule was successfully unapplied, otherwise <c>false</c></returns>
			public bool Unapply(WordAnalysis input, out ICollection<WordAnalysis> output)
			{
				VariableValues instantiatedVars = new VariableValues(m_alphaVars);
				IList<Match> matches;
				m_transform.RHSTemplate.IsMatch(input.Shape.First, Direction.RIGHT, ModeType.ANALYSIS, instantiatedVars, out matches);

				List<WordAnalysis> outputList = new List<WordAnalysis>();
				output = outputList;
				foreach (Match match in matches)
				{
					PhoneticShape headShape;
					PhoneticShape nonHeadShape;
					UnapplyRHS(match, out headShape, out nonHeadShape);

					// for computational complexity reasons, we ensure that the non-head is a root, otherwise we assume it is not
					// a valid analysis and throw it away
					foreach (LexEntry.RootAllomorph allo in Morpheme.Stratum.SearchEntries(nonHeadShape))
					{
						// check to see if this is a duplicate of another output analysis, this is not strictly necessary, but
						// it helps to reduce the search space
						bool add = true;
						for (int i = 0; i < output.Count; i++)
						{
							if (headShape.Duplicates(outputList[i].Shape) && allo == outputList[i].NonHead.RootAllomorph)
							{
								if (headShape.Count > outputList[i].Shape.Count)
									// if this is a duplicate and it is longer, then use this analysis and remove the previous one
									outputList.RemoveAt(i);
								else
									// if it is shorter, then do not add it to the output list
									add = false;
								break;
							}
						}

						if (add)
						{
							WordAnalysis wa = input.Clone();
							wa.Shape = headShape;
							wa.NonHead = new WordAnalysis(nonHeadShape, wa.Stratum, null);
							wa.NonHead.RootAllomorph = allo;
							output.Add(wa);
						}
					}
				}

				return outputList.Count > 0;
			}

			void UnapplyRHS(Match match, out PhoneticShape headShape, out PhoneticShape nonHeadShape)
			{
				headShape = new PhoneticShape();
				headShape.Add(new Margin(Direction.LEFT));
				nonHeadShape = new PhoneticShape();
				nonHeadShape.Add(new Margin(Direction.LEFT));
				// iterate thru LHS partitions, copying the matching partition from the
				// input to the output
				for (int i = 0; i < m_transform.PartitionCount; i++)
				{
					PhoneticShape curShape = i < m_firstNonHeadPartition ? headShape : nonHeadShape;
					m_transform.Unapply(match, i, curShape);
				}
				headShape.Add(new Margin(Direction.RIGHT));
				nonHeadShape.Add(new Margin(Direction.RIGHT));
			}

			/// <summary>
			/// Applies this subrule to the specified word synthesis.
			/// </summary>
			/// <param name="input">The input word synthesis.</param>
			/// <param name="output">The output word synthesis.</param>
			/// <returns><c>true</c> if the subrule was successfully applied, otherwise <c>false</c></returns>
			public bool Apply(WordSynthesis input, out WordSynthesis output)
			{
				output = null;

				// check MPR features
				if ((m_requiredMPRFeatures != null && m_requiredMPRFeatures.Count > 0 && !m_requiredMPRFeatures.IsMatch(input.MPRFeatures))
					|| (m_excludedMPRFeatures != null &&  m_excludedMPRFeatures.Count > 0 && m_excludedMPRFeatures.IsMatch(input.MPRFeatures)))
					return false;

				VariableValues instantiatedVars = new VariableValues(m_alphaVars);
				IList<Match> headMatches, nonHeadMatches;
				if (m_headLhsTemp.IsMatch(input.Shape.First, Direction.RIGHT, ModeType.SYNTHESIS, instantiatedVars, out headMatches)
					&& m_nonHeadLhsTemp.IsMatch(input.NonHead.Shape.First, Direction.RIGHT, ModeType.SYNTHESIS, instantiatedVars, out nonHeadMatches))
				{
					output = input.Clone();
					ApplyRHS(headMatches[0], nonHeadMatches[0], input, output);

					if (m_outputMPRFeatures != null)
						output.MPRFeatures.AddOutput(m_outputMPRFeatures);
					return true;
				}

				return false;
			}


			void ApplyRHS(Match headMatch, Match nonHeadMatch, WordSynthesis input, WordSynthesis output)
			{
				output.Shape.Clear();
				output.Morphs.Clear();
				output.Shape.Add(new Margin(Direction.LEFT));
				foreach (MorphologicalOutput outputMember in m_transform.RHS)
				{
					Match curMatch;
					WordSynthesis curInput;
					if (outputMember.Partition < m_firstNonHeadPartition)
					{
						curMatch = headMatch;
						curInput = input;
					}
					else
					{
						curMatch = nonHeadMatch;
						curInput = input.NonHead;
					}
					outputMember.Apply(curMatch, curInput, output, m_morpheme.Gloss != null ? this : null);
				}
				output.Shape.Add(new Margin(Direction.RIGHT));
			}
		}

		List<Subrule> m_subrules;

		HCObjectSet<PartOfSpeech> m_headRequiredPOSs = null;
		HCObjectSet<PartOfSpeech> m_nonHeadRequiredPOSs = null;
		PartOfSpeech m_outPOS = null;
		int m_maxNumApps = 1;
		FeatureValues m_headRequiredHeadFeatures = null;
		FeatureValues m_headRequiredFootFeatures = null;
		FeatureValues m_nonHeadRequiredHeadFeatures = null;
		FeatureValues m_nonHeadRequiredFootFeatures = null;
		FeatureValues m_outHeadFeatures = null;
		FeatureValues m_outFootFeatures = null;
		HCObjectSet<Feature> m_obligHeadFeatures = null;
		// TODO: add subcats

		/// <summary>
		/// Initializes a new instance of the <see cref="MorphologicalRule"/> class.
		/// </summary>
		/// <param name="id">The id.</param>
		/// <param name="desc">The description.</param>
		/// <param name="morpher">The morpher.</param>
		public CompoundingRule(string id, string desc, Morpher morpher)
			: base(id, desc, morpher)
		{
			m_subrules = new List<Subrule>();
		}

		/// <summary>
		/// Gets the maximum number of allowable applications of this rule.
		/// </summary>
		/// <value>The maximum number of applications.</value>
		public int MaxNumApps
		{
			get
			{
				return m_maxNumApps;
			}

			set
			{
				m_maxNumApps = value;
			}
		}

		/// <summary>
		/// Gets or sets the required parts of speech.
		/// </summary>
		/// <value>The required parts of speech.</value>
		public IEnumerable<PartOfSpeech> HeadRequiredPOSs
		{
			get
			{
				return m_headRequiredPOSs;
			}

			set
			{
				m_headRequiredPOSs = new HCObjectSet<PartOfSpeech>(value);
			}
		}

		/// <summary>
		/// Gets or sets the required parts of speech.
		/// </summary>
		/// <value>The required parts of speech.</value>
		public IEnumerable<PartOfSpeech> NonHeadRequiredPOSs
		{
			get
			{
				return m_nonHeadRequiredPOSs;
			}

			set
			{
				m_nonHeadRequiredPOSs = new HCObjectSet<PartOfSpeech>(value);
			}
		}

		/// <summary>
		/// Gets or sets the output part of speech.
		/// </summary>
		/// <value>The output part of speech.</value>
		public PartOfSpeech OutPOS
		{
			get
			{
				return m_outPOS;
			}

			set
			{
				m_outPOS = value;
			}
		}

		/// <summary>
		/// Gets or sets the required head features.
		/// </summary>
		/// <value>The required head features.</value>
		public FeatureValues HeadRequiredHeadFeatures
		{
			get
			{
				return m_headRequiredHeadFeatures;
			}

			set
			{
				m_headRequiredHeadFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the required foot features.
		/// </summary>
		/// <value>The required foot features.</value>
		public FeatureValues HeadRequiredFootFeatures
		{
			get
			{
				return m_headRequiredFootFeatures;
			}

			set
			{
				m_headRequiredFootFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the required head features.
		/// </summary>
		/// <value>The required head features.</value>
		public FeatureValues NonHeadRequiredHeadFeatures
		{
			get
			{
				return m_nonHeadRequiredHeadFeatures;
			}

			set
			{
				m_nonHeadRequiredHeadFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the required foot features.
		/// </summary>
		/// <value>The required foot features.</value>
		public FeatureValues NonHeadRequiredFootFeatures
		{
			get
			{
				return m_nonHeadRequiredFootFeatures;
			}

			set
			{
				m_nonHeadRequiredFootFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the output head features.
		/// </summary>
		/// <value>The output head features.</value>
		public FeatureValues OutHeadFeatures
		{
			get
			{
				return m_outHeadFeatures;
			}

			set
			{
				m_outHeadFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the output foot features.
		/// </summary>
		/// <value>The output foot features.</value>
		public FeatureValues OutFootFeatures
		{
			get
			{
				return m_outFootFeatures;
			}

			set
			{
				m_outFootFeatures = value;
			}
		}

		/// <summary>
		/// Gets or sets the obligatory head features.
		/// </summary>
		/// <value>The obligatory head features.</value>
		public IEnumerable<Feature> ObligatoryHeadFeatures
		{
			get
			{
				return m_obligHeadFeatures;
			}

			set
			{
				m_obligHeadFeatures = new HCObjectSet<Feature>(value);
			}
		}

		/// <summary>
		/// Gets the subrules.
		/// </summary>
		/// <value>The subrules.</value>
		public IEnumerable<Subrule> Subrules
		{
			get
			{
				return m_subrules;
			}
		}

		/// <summary>
		/// Gets the subrule count.
		/// </summary>
		/// <value>The subrule count.</value>
		public override int SubruleCount
		{
			get
			{
				return m_subrules.Count;
			}
		}

		/// <summary>
		/// Adds a subrule.
		/// </summary>
		/// <param name="sr">The subrule.</param>
		public void AddSubrule(Subrule sr)
		{
			sr.Morpheme = this;
			sr.Index = m_subrules.Count;
			m_subrules.Add(sr);
		}

		/// <summary>
		/// Performs any pre-processing required for unapplication of a word analysis. This must
		/// be called before <c>Unapply</c>. <c>Unapply</c> and <c>EndUnapplication</c> should only
		/// be called if this method returns <c>true</c>.
		/// </summary>
		/// <param name="input">The input word analysis.</param>
		/// <returns>
		/// 	<c>true</c> if the specified input is unapplicable, otherwise <c>false</c>.
		/// </returns>
		public override bool BeginUnapplication(WordAnalysis input)
		{
			return input.GetNumUnappliesForMorphologicalRule(this) < m_maxNumApps && input.NonHead == null
				&& (m_outPOS == null || input.MatchPOS(m_outPOS));
		}

		/// <summary>
		/// Unapplies the specified subrule to the specified word analysis.
		/// </summary>
		/// <param name="input">The input word analysis.</param>
		/// <param name="srIndex">Index of the subrule.</param>
		/// <param name="output">All resulting word analyses.</param>
		/// <returns>
		/// 	<c>true</c> if the subrule was successfully unapplied, otherwise <c>false</c>
		/// </returns>
		public override bool Unapply(WordAnalysis input, int srIndex, out ICollection<WordAnalysis> output)
		{
			if (m_subrules[srIndex].Unapply(input, out output))
			{
				foreach (WordAnalysis wa in output)
				{
					if (m_headRequiredPOSs != null && m_headRequiredPOSs.Count > 0)
					{
						foreach (PartOfSpeech pos in m_headRequiredPOSs)
							wa.AddPOS(pos);
					}
					else if (m_outPOS == null)
					{
						wa.UninstantiatePOS();
					}

					if (m_nonHeadRequiredPOSs != null)
					{
						foreach (PartOfSpeech pos in m_nonHeadRequiredPOSs)
							wa.NonHead.AddPOS(pos);
					}

					wa.MorphologicalRuleUnapplied(this);

					if (TraceAnalysis)
					{
						// create the morphological rule analysis trace record for each output analysis
						MorphologicalRuleAnalysisTrace trace = new MorphologicalRuleAnalysisTrace(this, input.Clone());
						trace.RuleAllomorph = m_subrules[srIndex];
						trace.Output = wa.Clone();
						wa.CurrentTrace.AddChild(trace);
						// set current trace record to the morphological rule trace record for each
						// output analysis
						wa.CurrentTrace = trace;
					}
				}
				return true;
			}

			output = null;
			return false;
		}

		/// <summary>
		/// Performs any post-processing required after the unapplication of a word analysis. This must
		/// be called after a successful <c>BeginUnapplication</c> call and any <c>Unapply</c> calls.
		/// </summary>
		/// <param name="input">The input word analysis.</param>
		/// <param name="unapplied">if set to <c>true</c> if the input word analysis was successfully unapplied.</param>
		public override void EndUnapplication(WordAnalysis input, bool unapplied)
		{
			if (TraceAnalysis && !unapplied)
				// create the morphological rule analysis trace record for a rule that did not succesfully unapply
				input.CurrentTrace.AddChild(new MorphologicalRuleAnalysisTrace(this, input.Clone()));
		}

		/// <summary>
		/// Determines whether this rule is applicable to the specified word synthesis.
		/// </summary>
		/// <param name="input">The input word synthesis.</param>
		/// <returns>
		/// 	<c>true</c> if the rule is applicable, otherwise <c>false</c>.
		/// </returns>
		public override bool IsApplicable(WordSynthesis input)
		{
			// TODO: check subcats.

			// check required parts of speech
			return input.NextRule == this && input.GetNumAppliesForMorphologicalRule(this) < m_maxNumApps
				&& (m_headRequiredPOSs == null || m_headRequiredPOSs.Count == 0 || m_headRequiredPOSs.Contains(input.POS))
				&& input.NonHead != null
				&& (m_nonHeadRequiredPOSs == null || m_nonHeadRequiredPOSs.Count == 0 || m_nonHeadRequiredPOSs.Contains(input.NonHead.POS));
		}

		/// <summary>
		/// Applies the rule to the specified word analysis.
		/// </summary>
		/// <param name="input">The input word synthesis.</param>
		/// <param name="output">The output word syntheses.</param>
		/// <returns>
		/// 	<c>true</c> if the rule was successfully applied, otherwise <c>false</c>
		/// </returns>
		public override bool Apply(WordSynthesis input, out ICollection<WordSynthesis> output)
		{
			output = null;

			// these should probably be moved to IsApplicable, but we will leave it here for
			// now so we don't have to call it again to set the features for the output word
			// synthesis record

			// check head features
			FeatureValues headHeadFeatures;
			if (!m_headRequiredHeadFeatures.UnifyDefaults(input.HeadFeatures, out headHeadFeatures))
				return false;

			FeatureValues nonHeadHeadFeatures;
			if (!m_nonHeadRequiredHeadFeatures.UnifyDefaults(input.NonHead.HeadFeatures, out nonHeadHeadFeatures))
				return false;

			// check foot features
			FeatureValues headFootFeatures;
			if (!m_headRequiredFootFeatures.UnifyDefaults(input.FootFeatures, out headFootFeatures))
				return false;

			FeatureValues nonHeadFootFeatures;
			if (!m_nonHeadRequiredFootFeatures.UnifyDefaults(input.NonHead.FootFeatures, out nonHeadFootFeatures))
				return false;

			MorphologicalRuleSynthesisTrace trace = null;
			if (TraceSynthesis)
			{
				// create morphological rule synthesis trace record
				trace = new MorphologicalRuleSynthesisTrace(this, input.Clone());
				input.CurrentTrace.AddChild(trace);
			}

			output = new List<WordSynthesis>();
			foreach (Subrule sr in m_subrules)
			{
				WordSynthesis ws;
				if (sr.Apply(input, out ws))
				{
					if (m_outPOS != null)
						ws.POS = m_outPOS;

					if (m_outHeadFeatures != null)
						ws.HeadFeatures = m_outHeadFeatures.Clone();

					ws.HeadFeatures.Add(headHeadFeatures);

					if (m_outFootFeatures != null)
						ws.FootFeatures = m_outFootFeatures.Clone();

					ws.FootFeatures.Add(headFootFeatures);

					if (m_obligHeadFeatures != null)
					{
						foreach (Feature feature in m_obligHeadFeatures)
							ws.AddObligatoryHeadFeature(feature);
					}

					ws.MorphologicalRuleApplied(this);

					ws = CheckBlocking(ws);

					if (trace != null)
					{
						// set current trace record to the morphological rule trace record for each
						// output analysis
						ws.CurrentTrace = trace;
						// add output to morphological rule trace record
						trace.RuleAllomorph = sr;
						trace.Output = ws.Clone();
					}

					output.Add(ws);
					// return all word syntheses that match subrules that are constrained by environments,
					// HC violates the disjunctive property of allomorphs here because it cannot check the
					// environmental constraints until it has a surface form, we will enforce the disjunctive
					// property of allomorphs at that time
					if (sr.RequiredEnvironments == null && sr.ExcludedEnvironments == null)
					{
						break;
					}
				}
			}

			return output.Count > 0;
		}

		/// <summary>
		/// Applies the rule to the specified word synthesis. This method is used by affix templates.
		/// </summary>
		/// <param name="input">The input word synthesis.</param>
		/// <param name="origHeadFeatures">The original head features before template application.</param>
		/// <param name="output">The output word syntheses.</param>
		/// <returns>
		/// 	<c>true</c> if the rule was successfully applied, otherwise <c>false</c>
		/// </returns>
		public override bool ApplySlotAffix(WordSynthesis input, FeatureValues origHeadFeatures, out ICollection<WordSynthesis> output)
		{
			return Apply(input, out output);
		}

		public override void Reset()
		{
			base.Reset();

			m_headRequiredPOSs = null;
			m_nonHeadRequiredPOSs = null;
			m_outPOS = null;
			m_maxNumApps = 1;
			m_headRequiredHeadFeatures = null;
			m_headRequiredFootFeatures = null;
			m_nonHeadRequiredHeadFeatures = null;
			m_nonHeadRequiredFootFeatures = null;
			m_outHeadFeatures = null;
			m_outFootFeatures = null;
			m_obligHeadFeatures = null;
			m_subrules.Clear();
		}
	}
}